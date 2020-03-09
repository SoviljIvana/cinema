import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Container, ToggleButton, ToggleButtonGroup, Button, Card } from 'react-bootstrap';
import './App.css';
import jwt_decode from 'jwt-decode';
var decoded = jwt_decode(localStorage.getItem('jwt'));
console.log(decoded);
var userNameFromJWT = decoded.sub;
console.log(userNameFromJWT)


class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            seats: [],
            isLoading: true,
            button: true,
            tickets: [],
            listOfSeats: [],
            projectionId: '',
            submitted: false,
            projections: []
        };
        this.handleClick = this.handleClick.bind(this);
        this.addTickets = this.addTickets.bind(this);
    }

    componentDidMount() {
        var idProjection = window.location.pathname.split("/").pop();
        this.getSeats(idProjection);
        this.state.projectionId = idProjection;
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
                        seats: data,
                        isLoading: false
                    });
                }
                // console.log(data);
            })
            .catch(response => {
                this.setState({ isLoading: false });
                NotificationManager.error(response.message || response.statusText);
            });
    }

    addTickets() {
        const { listOfSeats, projectionId } = this.state;

        const data = {
            seatModels: listOfSeats,
            ProjectionId: projectionId,
            UserName: userNameFromJWT,
        };

        const requestOptions = {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            },
            body: JSON.stringify(data)

        };

        fetch(`${serviceConfig.baseURL}/api/Tickets/add`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.json();
            })
            .catch(response => {
                NotificationManager.error(response.message || response.statusText);
                this.setState({ subitted: false });

            });

    }

    handleClick(seat) {
        seat.counter = seat.counter + 1;
        if (!seat.reserved) {

            if (seat.counter % 2 != 0) {

                if (this.state.listOfSeats.length == 0) {
                    this.state.listOfSeats.push(seat.id);
                }
                else {
                    let n = 0;
                    for (let index = 0; index < this.state.listOfSeats.length; index++) {
                        if (this.state.listOfSeats[index] == seat.id) {
                            this.state.listOfSeats.splice(index, 1)
                            n = n + 1;
                        }
                    }
                    if (n == 0) {
                        this.state.listOfSeats.push(seat.id);
                        this.setState({
                            button: !this.state.button
                        })
                    }
                }

            }
        }
    }

    renderRowsInProjections(seatsInRow) {
        return seatsInRow.map((seat) => {
            return <ToggleButtonGroup type="checkbox"> <ToggleButton type="button" disabled={seat.reserved === true ? true : false} className={this.state.button ? "buttonTrue" : "buttonFalse"} onClick={() => this.handleClick(seat)}>{seat.row},{seat.number}</ToggleButton></ToggleButtonGroup>
        })
    }

    fillTableWithDaata() {
        return this.state.seats.map(seat => {
            return <Row className="justify-content-center" style={{ width: '100rem' }}>{this.renderRowsInProjections(seat.seatsInRow)}<br></br></Row>
        })
    }

    render() {
        const rowsData = this.fillTableWithDaata();
        console.log();
        return (
            <Container>
                <Row className="justify-content-center">
                    <br></br>
                    {rowsData}
                    <br></br>
                </Row>
                <Link className="text-decoration-none" to='/tickets'>  <Button className="justify-content-center" onClick={this.addTickets} > Create ticket </Button></Link>
            </Container>
        );
    }
}
export default ProjectionDetails;
