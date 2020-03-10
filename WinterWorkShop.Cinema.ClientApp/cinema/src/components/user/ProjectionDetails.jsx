import React, { Component } from 'react';
import { Link } from 'react-router-dom';
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
                    if (seat.row == this.state.listOfSeats[0].row) // provera reda u kom se nallazi element
                    {
                        let length = this.state.listOfSeats.length // definisanje duzine liste
                        let first = this.state.listOfSeats[length - 1].number; // poslednji element
                        let second = 1;
                        if (this.state.listOfSeats.length >= 2) { // ako je u listi vise od 2 elementa

                            second = this.state.listOfSeats[length - 2].number; // pretposlednji element
                            let n = 0; // broj ponavljanja elementa koji se dodaje u listu 
                            for (let index = 0; index < this.state.listOfSeats.length; index++) //prolazi kroz listu elemenata
                            {
                                if (this.state.listOfSeats[index].id == seat.id) // proverava da li ima jednakih elemenata
                                {
                                    if (seat.number + 1 == first || seat.number - 1 == first || second == seat.number + 1 || second == seat.number - 1) {
                                        this.state.listOfSeats.splice(index, 1)
                                        //element.classList.add(buttonTrue);
                                        element.classList.remove(buttonFalse);
                                        n = n + 1;
                                    }
                                }
                            }
                            if (n == 0) {
                                if (first == seat.number + 1 || first == seat.number - 1 || second == seat.number + 1 || second == seat.number - 1) {
                                    this.state.listOfSeats.push(seat);
                                    element.classList.add(buttonFalse);

                                }
                                else {
                                    //element.classList.add(buttonTrue);
                                    element.classList.remove(buttonFalse);
                                    return NotificationManager.error("mora biti jedan pored drugog!");
                                }
                            }
                        }
                        else // ako je u listi samo jedan element 
                        {
                            if (this.state.listOfSeats[0].id == seat.id) {
                                this.state.listOfSeats.splice(0, 1)
                                //element.classList.add(buttonTrue);
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

                        // this.handleColor(seat);

                        return NotificationManager.error("mora isti red!");
                        
                    }
                }
            //}
        }
        console.log(this.state.listOfSeats);
    }

    // handleColor(seat){
        
    //                             // let ida = 'ab348100-b076-4417-a70c-08d7c4d412ee';
    //                             let element = document.getElementById(id);
    //                             let newClass = "buttonTrue1";
    //                             let primary = "btn-primary";
                                
    //                             element.classList.add(newClass);
        
    //                              console.log(element.classList);

    // }

    renderRowsInProjections(seatsInRow) {
        return seatsInRow.map((seat) => {

            return <Button type="button" id={seat.id}
                disabled={seat.reserved === true ? true : false} 
                onClick={() => this.handleClick(seat)}>{seat.row},{seat.number}</Button>
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
                <Link className="text-decoration-none" to='/tickets'>  <Button onClick={this.addTickets} > Create ticket </Button></Link>

            </Container>
        );
    }
}
export default ProjectionDetails;
