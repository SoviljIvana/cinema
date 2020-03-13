import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { MDBBtn, MDBModal, MDBRow, MDBCol } from 'mdbreact'
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Container, ToggleButton, ToggleButtonGroup, Button, Card } from 'react-bootstrap';
import './App.css';
import jwt_decode from 'jwt-decode';
import $ from "jquery";
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
        // this.handleColor = this.handleColor.bind(this);
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
        fetch(`${serviceConfig.baseURL}/api/Seats/allForProjection/` + projectionId, requestOptions)
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

           // if (seat.counter % 2 != 0) { //dupli klik handle
                let id = seat.id
                let element = document.getElementById(id);
                let buttonTrue = "buttonTrue";
                let buttonFalse = "buttonFalse";


                //element.classList.remove(buttonTrue);
                
                 if (this.state.listOfSeats.length == 0) //provera da li je lista prazna, ako jeste dodaje element
                {
                    this.state.listOfSeats.push(seat);
                    element.classList.add(buttonFalse);

                }
                else {

                    if (seat.row == this.state.listOfSeats[0].row) 
                    {
                        let length = this.state.listOfSeats.length 
                        let first = this.state.listOfSeats[length - 1].number; 
                        let second = 1;
                        if (this.state.listOfSeats.length >= 2) { 

                            let maxNumber = 1;
                            let minNumber = 1;
                            for (let index = 0; index < this.state.listOfSeats.length; index++) {
                                if(this.state.listOfSeats[index].number > maxNumber){
                                    maxNumber = this.state.listOfSeats[index].number;
                                }
                                if(this.state.listOfSeats[index].number < minNumber){
                                    minNumber = this.state.listOfSeats[index].number;
                                }
                            }

                            second = this.state.listOfSeats[length - 2].number; 
                            let n = 0; 
                            for (let index = 0; index < this.state.listOfSeats.length; index++) 
                            {
                                if (this.state.listOfSeats[index].id == seat.id) 
                                {
                                    n = n + 1;
                                    if (seat.number == maxNumber || seat.number == minNumber ) {
                                        this.state.listOfSeats.splice(index, 1)
                                        element.classList.remove(buttonFalse);
                                        
                                    }
                                }
                            }
                            if (n == 0) {
                                if (seat.number - 1 == maxNumber || seat.number + 1 == minNumber ) {
                                     this.state.listOfSeats.push(seat);
                                    element.classList.add(buttonFalse);

                                }
                                else {
                                    return NotificationManager.error("mora biti jedan pored drugog!");
                                }
                            }
                            else{
                                if (seat.number < maxNumber && seat.number > minNumber ) {
                                    return NotificationManager.error("mora biti krajnji!");
                                   
                                    }
                            }
                        }
                        else 
                        {
                            if (this.state.listOfSeats[0].id == seat.id) {
                                this.state.listOfSeats.splice(0, 1)
                                element.classList.remove(buttonFalse)
                            }
                           else if (this.state.listOfSeats[0].number == seat.number + 1 || this.state.listOfSeats[0].number == seat.number - 1) {
                                this.state.listOfSeats.push(seat);
                                element.classList.add(buttonFalse);

                            }
                            else {
                                return NotificationManager.error("mora biti jedan pored drugog!");
                            }
                        }
                    }
                    else{
                        return NotificationManager.error("mora isti red!");
                    }
                }
            //}
        }
        console.log(this.state.listOfSeats);
    }

    renderRowsInProjections(seatsInRow) {
        return seatsInRow.map((seat) => {

            return <MDBBtn className="circle"  color="danger" outline="true" rounded="true"  mdbWavesEffect  type="button" id={seat.id}
                disabled={seat.reserved === true ? true : false} 
                onClick={() => this.handleClick(seat)}>+</MDBBtn>
        })
    }

    fillTableWithDaata() {
        return this.state.seats.map(seat => {
            return <MDBRow className="justify-content-center" style={{ width: '100rem' }}>{this.renderRowsInProjections(seat.seatsInRow)}<br></br></MDBRow>
        })
    }

    render() {
        const rowsData = this.fillTableWithDaata();
        console.log();
        return (
            <Container>
                <Row className="justify-content-center">
                    {rowsData}
                    <br></br>
                    <Link className="text-decoration-none" to='/tickets'> <Button variant="dark" onClick={this.addTickets} > Create ticket </Button></Link>
                </Row>
            </Container>
        );
    }
}
export default ProjectionDetails;
