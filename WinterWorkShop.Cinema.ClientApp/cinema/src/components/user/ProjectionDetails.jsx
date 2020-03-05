import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormGroup, Row, Col, Table, Button, ToggleButton, ToggleButtonGroup } from 'react-bootstrap';
import './App.css';
import $ from 'jquery';

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            seats: [],
            isLoading: true,
            button: true,
            count: 1
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
            })
            .catch(response => {
                this.setState({ isLoading: false });
                NotificationManager.error(response.message || response.statusText);
            });
    }
    
    handleClick() {
        this.setState({
            button: !this.state.button
        })
    }

    renderRowsInProjections(seatsInRow) {
        return seatsInRow.map((seat) => { return <ToggleButtonGroup type="checkbox"> <ToggleButton type="button" disabled={seat.reserved === true ? true : false} className={this.state.button ? "buttonTrue" : "buttonFalse"} onClick={this.handleClick}>{seat.row},{seat.number}</ToggleButton></ToggleButtonGroup> })
    }

    fillTableWithDaata() {
        return this.state.seats.map(seat => {
            return <Row className="justify-content-center" style={{ width: '100rem' }}>{this.renderRowsInProjections(seat.seatsInRow)}<br></br></Row>
        })
    }

    render() {
        const rowsData = this.fillTableWithDaata();
        return (
            <Row className="justify-content-center">
                <br></br>
                {rowsData}
                <br></br>
            </Row>
        );
    }
}

export default ProjectionDetails;