import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormGroup, Row, Col, Table, Button } from 'react-bootstrap';
import Spinner from '../Spinner';
import ReactStars from 'react-stars';
import './App.css';

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);

        this.state = {
            projections: [],
            seats: [],
            isLoading: true,
            id: '',
            title: '',
            year: 0,
            rating: '',
            movieId: '',
            auditoriumId: '',
            current: false,
            titleError: '',
            yearError: '',
            submitted: false,
            canSubmit: true,
            black: true,
            button: true,
            auditoriumId: '',
            row: '',
            number: '',
        };
        this.handleSubmit = this.handleSubmit.bind(this);
        this.details = this.details.bind(this);
    }

    componentDidMount() {
        const { id } = this.props.match.params;
        this.getProjections(id);
        this.getMovie(id);

    }

    getProjections(movieId) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        this.setState({ isLoading: true });
        fetch(`${serviceConfig.baseURL}/api/Movies/allForSpecificMovie/` + movieId, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    this.setState({
                        projections: data,
                        isLoading: false
                    });
                }
            })
            .catch(response => {
                this.setState({ isLoading: false });
                NotificationManager.error(response.message || response.statusText);
            });
    }

    getSeats(projectionId) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        this.setState({ isLoading: true });
        fetch(`${serviceConfig.baseURL}/api/Movies/allForProjection/` + projectionId, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    this.setState({
                        projections: data,
                        isLoading: false
                    });
                }
            })
            .catch(response => {
                this.setState({ isLoading: false });
                NotificationManager.error(response.message || response.statusText);
            });
    }


    getMovie(movieId) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/movies/` + movieId, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    this.setState({
                        title: data.title,
                        year: data.year,
                        rating: Math.round(data.rating) + '',
                        current: data.current + '',
                        id: data.id
                    });
                }
            })
            .catch(response => {
                NotificationManager.error(response.message || response.statusText);
                this.setState({ submitted: false });
            });
    }

    details(id) {

        this.props.history.push(`allForProjection/` + `${id}`);
        this.getSeats(id);
    }


    handleSubmit(e) {
        e.preventDefault();
        this.setState({ submitted: true });
        const { title, year, rating } = this.state;
        if (title && year && rating) {
            this.updateMovie();
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }


    fillTableWithDaata() {

        return this.state.projections.map(projection => {

            return <Button key={projection.id} className="mr-1 mb-2">
                <br></br>
                {
                    <Button>
                        <h3><button onClick={() => this.details(projection.id)}>
                            <header>{projection.projectionTimeString}</header></button> </h3>
                        <br></br>
                        <div>
                            <h3 className="form-header">{projection.auditoriumName} </h3>
                        </div>
                        <br></br>
                        <br></br>
                        <button>
                            {projection.id}
                        </button>
                        <div>
                            {projection.auditoriumId}
                        </div>
                        <div>
                            {projection.row}
                        </div>
                        <div>
                            {projection.number}
                        </div>
                        <div>
                            {this.renderRows(projection.row, projection.number)}
                        </div>
                        <tbody>
                            {this.renderRows(projection.numOFRows, projection.numOFSeatsPerRow, projection.id)}
                        </tbody>
                    </Button>

                }
            </Button>
        })
    }

    renderRows(rows, seats) {
        const rowsRendered = [];
        for (let i = 0; i < rows; i++) {
            rowsRendered.push(<tr key={i}> {this.renderSeats(seats, i)}</tr>);
        }
        return rowsRendered;
    }

    renderSeats(seats, row) {
        let renderedSeats = [];
        for (let i = 0; i < seats; i++) {
            renderedSeats.push(<button key={'row: ' + row + ', seat: ' + i}>{row}, {i}</button>);
        }
        return renderedSeats;
    }

    render() {
        const { isLoading, title, year, rating } = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table striped bordered hover size="sm" variant="link">
            <tbody>
                {rowsData}
            </tbody>

        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
        return (
            <React.Fragment>
                <Row className="justify-content-center">
                    <Col>
                        <br></br>
                        <h1>Title: {title} </h1>
                        <h2>Year: {year} </h2>
                        <h3>Rating: <FormGroup> <ReactStars count={10} edit={false} size={37} value={rating} color1={'grey'} color2={'#ffd700'} /></FormGroup>
                        </h3>
                        <FormGroup >
                            <br></br>
                            {showTable}
                        </FormGroup>
                        <hr />
                    </Col>
                </Row>
            </React.Fragment>
        );
    }
}
export default ProjectionDetails;
