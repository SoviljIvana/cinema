import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormGroup, Row, Col, Table, Button } from 'react-bootstrap';
import Spinner from '../Spinner';
import ReactStars from 'react-stars';
import './App.css';
class AllForProjection extends Component {

    constructor(props) {
        super(props);

        this.state = {
            projectionId: '',
            id: '',
            auditoriumId: '',
            row: '',
            number: '',
        };
    }

    componentDidMount() {
        const { projectionId } = this.props.match.params; 
        this.getSeats(projectionId);
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

    render() {
        const { id, auditoriumId, row, number } = this.state;
        return (
            <React.Fragment>
                <Row className="justify-content-center">
                    <Col>
                        <h1>SEAT ID: {id} </h1>
                        <h2>AUDITORIUM ID: {auditoriumId} </h2>
                        <h2>ROW: {row} </h2>
                        <h2>NUMBER: {number} </h2>
                    </Col>
                </Row>
            </React.Fragment>
        );
    }
}
export default AllForProjection;

