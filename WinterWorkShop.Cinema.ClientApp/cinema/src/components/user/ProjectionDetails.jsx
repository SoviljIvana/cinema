import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormGroup, Row, Col, Table, Button} from 'react-bootstrap';
import Spinner from '../Spinner';
import ReactStars from 'react-stars';
import './App.css';

class ProjectionDetails extends Component {

    constructor(props) {
        super(props);
        this.state = {
            seats: [],
            isLoading: true,
            button: true,
            count: 1
        };
        //this.fillTableWithDaata = this.fillTableWithDaata.bind(this);
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

    renderRowsInProjections(seatsInRow) {

        return seatsInRow.map((seat) => {
            
          return <Button>{seat.row},{seat.number}</Button>
          
        })
        
      }

    fillTableWithDaata() {

                return this.state.seats.map(seat => {
            return <Row className="justify-content-center" style={{ width: '100rem'}}>{this.renderRowsInProjections(seat.seatsInRow)}<br></br></Row>
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
