import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import ReactDOM from 'react-dom';
import { serviceConfig } from '../../appSettings';
import { FormGroup, FormControl, Button, Container, Row, Col, FormText, FormLabel, Alert, Table } from 'react-bootstrap';
import Spinner from '../Spinner';
import ReactStars from 'react-stars';
import './App.css';
class ProjectionDetails extends Component {

    constructor(props) {
        super(props);

        this.state = {
            projections: [],
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
        this.handleClick = this.handleClick.bind(this);
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
            return <tr key={projection.id, projection.movieId} className="mr-1 mb-2">
                <br></br>
                {
                    <card className="table-cinema-auditorium" >
                        <h3> Time: <button > <header>{projection.projectionTimeString}</header></button> </h3>
                        <br></br>
                        <div>
                            <h3 className="form-header">Auditorium name:{projection.auditoriumName} </h3>

                        </div>
                        <tbody>
                            {this.renderRows(projection.numOFRows, projection.numOFSeatsPerRow, projection.id)}
                        </tbody>
                        <br></br>
                        <br></br>
                    </card>
                }
                <br>
                </br>
            </tr>
        })
    }

    handleClick() {
        this.setState({
            button: !this.state.button
        })
    }

    renderRows(rows, seats) {
        const rowsRendered = [];
        for (let i = 1; i < rows; i++) {
            rowsRendered.push(<tr key={i}> {this.renderSeats(seats, i)}</tr>);
        }
        return rowsRendered;
    }

    renderSeats(seats, row) {
        let renderedSeats = [];
        for (let i = 1; i < seats; i++) {
            renderedSeats.push(<button key={'row: ' + row + ', seat: ' + i} className={this.state.button ? "buttonTrue" : "buttonFalse"} onClick={this.handleClick}>{row}{i}</button>);
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
