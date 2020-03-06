import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Container, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import './App.css';
import $ from 'jquery';
var LinkedList = require('linked-list-adt');
var list = new LinkedList();

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            seats: [],
            isLoading: true,
            button: true,
            listOfSeats: [],
        };
        this.handleClick = this.handleClick.bind(this);
    }

    componentDidMount() {
        var idProjection = window.location.pathname.split("/").pop();
        this.getSeats(idProjection);
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


    handleClick(seat) {
        seat.counter = seat.counter + 1;
        if (!seat.reserved) {
            
        if (seat.counter % 2 != 0) {
            
            if (this.state.listOfSeats.length == 0) {
                this.state.listOfSeats.push(seat.id);
            }
            else{
                let n = 0
                for (let index = 0; index < this.state.listOfSeats.length; index++) {
                    
                    if( this.state.listOfSeats[index] == seat.id){

                        this.state.listOfSeats.splice(index, 1)
                        n = n+ 1;
                    }
                    
                    // else{
                    //     this.state.listOfSeats.push(seat.id);
                    // }
                }
                if (n == 0) {
                    this.state.listOfSeats.push(seat.id);
                }
            }

            console.log(this.state.listOfSeats);

        }

        }
         

        // if (!seat.reserved) {
        //     let i = 0
        //     if (this.state.listOfSeats.length != 0) {
        //         for (let index = 0; index < this.state.listOfSeats.length; index++) {
                
        //             if( this.state.listOfSeats[index] == seat.id){
        //                 i = i+1;
        //             }
        //         }
        //         if (i==0) {
        //             this.state.listOfSeats.push(seat.id);
        //         }
        //     }else{
        //         this.state.listOfSeats.push(seat.id);
        //         seat.selected = true
        //     }
            
        //     
            
        // }
    }

    renderRowsInProjections(seatsInRow) {
        return seatsInRow.map((seat) => { return <ToggleButtonGroup type="checkbox"> <ToggleButton type="button" disabled={seat.reserved === true ? true : false} className={this.state.button ? "buttonTrue" : "buttonFalse"} onClick={() => this.handleClick(seat)}>{seat.row},{seat.number}  </ToggleButton></ToggleButtonGroup> })
    }


    fillTableWithDaata() {
        return this.state.seats.map(seat => {
            return <Row className="justify-content-center" style={{ width: '100rem' }}>{this.renderRowsInProjections(seat.seatsInRow)}<br></br></Row>
        })
    }

    render() {
        const rowsData = this.fillTableWithDaata();

        return (
            <Container>
                <Row className="justify-content-center">
                    <br></br>
                    {rowsData}
                    <br></br>
                </Row>
            </Container>
        );
    }
}

export default ProjectionDetails;