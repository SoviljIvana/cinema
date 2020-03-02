import React from 'react';
import { withRouter } from 'react-router-dom';
import { FormGroup, FormControl, Button, Container, Row, Col, FormText, } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { YearPicker } from 'react-dropdown-date';

class EditAuditorium extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: '',
            seatRows: 0,
            numberOfSeats: 0,
            id: '',            
            auditNameError: '',
            seatRowsError: '',
            numOfSeatsError: '',
            submitted: false,
            canSubmit: true
        };
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    componentDidMount() {
        const { id } = this.props.match.params;
        this.getAuditorium(id);
    }

    handleChange(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
        this.validate(id, value);
    }    

    handleSubmit(e) {

        e.preventDefault();
        this.setState({ submitted: true });
        const { name, numberOfSeats, seatRows } = this.state;
        if (name && numberOfSeats && seatRows) {
            this.updateAuditorium();
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }
    validate(id, value) {
        if (id === 'name') {
            if (value === '') {
                this.setState({
                    auditNameError: 'Fill in auditorium name',
                    canSubmit: false
                });
            } else {
                this.setState({
                    auditNameError: '',
                    canSubmit: true
                });
            }
        }else if (id === 'numberOfSeats') {
            const seatsNum = +value;
            if (seatsNum > 20 || seatsNum < 1) {
                this.setState({
                    numOfSeatsError: 'Seats number can be in between 1 and 20',
                    canSubmit: false
                })
            } else {
                this.setState({
                    numOfSeatsError: '',
                    canSubmit: true
                });
            }
        } else if (id === 'seatRows') {
            const seatsNum = +value;
            if (seatsNum > 20 || seatsNum < 1) {
                this.setState({
                    seatRowsError: 'Seats number can be in between 1 and 20',
                    canSubmit: false
                })
            } else {
                this.setState({
                    seatRowsError: '',
                    canSubmit: true
                });
            }
        } 
    }
    getAuditorium(auditoriumId) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/auditoriums/` + auditoriumId, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .then(data => {
                if (data) {
                    this.setState({
                        name: data.name,
                        seatRows: data.seatRows + '', 
                        numberOfSeats: data.numberOfSeats + '',
                        id: data.id
                    });
                }
            })
            .catch(response => {
                NotificationManager.error(response.message || response.statusText);
                this.setState({ submitted: false });
            });
    }

    updateAuditorium() {
        const { name, numberOfSeats, seatRows, id } = this.state;

        const data = {
            Name: name,
            NumberOfSeats: +numberOfSeats, 
            SeatRows: +seatRows       
        };

        const requestOptions = {
            method: 'PUT',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            },
            body: JSON.stringify(data)
        };

        console.log(JSON.stringify("REQ_OPT:" + requestOptions.body));
        
        fetch(`${serviceConfig.baseURL}/api/auditoriums/${id}`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.statusText;
            })
            .then(data => {
                if(data){
                    this.setState({
                        name : data.name,
                        seatRows: data.seatRows,
                        numberOfSeats: data.numberOfSeats,
                        id: data.id
                    });
                }
            })
            .then(result => {
                this.props.history.goBack();
                NotificationManager.success('Successfuly edited auditorium!');
            })
            .catch(response => {
                NotificationManager.error("Unable to update auditorium. ");
                this.setState({ submitted: false });
            });
    }

    renderRows(rows, seats) {
        const rowsRendered = [];
        for (let i = 0; i < rows; i++) {
            rowsRendered.push(<tr key={i}>
                {this.renderSeats(seats, i)}
            </tr>);
        }
        return rowsRendered;
    }

    renderSeats(seats, row) {
        let renderedSeats = [];
        for (let i = 0; i < seats; i++) {
            renderedSeats.push(<td key={'row: ' + row + ', seat: ' + i}></td>);
        }
        return renderedSeats;
    }

    render() {
        const {numberOfSeats, submitted, seatRows, name, auditNameError, numOfSeatsError,
            seatRowsError, canSubmit } = this.state;
        const auditorium = this.renderRows(seatRows, numberOfSeats);
        return (
            <Container>
                <Row>
                    <Col>
                        <h1 className="form-header">Edit Auditorium</h1>
                        <form onSubmit={this.handleSubmit}>
                            <FormGroup>
                                <FormControl
                                    id="name"
                                    type="text"
                                    placeholder="Auditorium Name"
                                    value={name}
                                    onChange={this.handleChange}
                                />

                                <FormText className="text-danger">{auditNameError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <FormControl
                                    id="seatRows"
                                    type="number"
                                    placeholder="Number Of Rows"
                                    value={seatRows}
                                    onChange={this.handleChange}
                                />
                                <FormText className="text-danger">{seatRowsError}</FormText>
                            </FormGroup>
                            <FormGroup>
                                <FormControl
                                    id="numberOfSeats"
                                    type="number"
                                    placeholder="Number Of Seats"
                                    value={numberOfSeats}
                                    onChange={this.handleChange}
                                    max="36"
                                />
                                <FormText className="text-danger">{numOfSeatsError}</FormText>
                            </FormGroup>
                            <Button type="submit" disabled={submitted || !canSubmit} block>Edit Auditorium</Button>
                        </form>
                    </Col>
                </Row>
                <Row className="mt-2">
                    <Col className="justify-content-center align-content-center">
                        <h1>Auditorium Preview</h1>
                        <div>
                            <Row className="justify-content-center mb-4">
                                <div className="text-center text-white font-weight-bold cinema-screen">
                                    CINEMA SCREEN
                            </div>
                            </Row>
                            <Row className="justify-content-center">
                                <table className="table-cinema-auditorium">
                                    <tbody>
                                        {auditorium}
                                    </tbody>
                                </table>
                            </Row>
                        </div>
                    </Col>
                </Row>
            </Container>
        );
    }
}
export default withRouter(EditAuditorium);