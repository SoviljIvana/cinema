import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { Table } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Switch from "react-switch";
import ReactStars from 'react-stars';
import { Navbar, Nav, Form, FormControl, Button} from 'react-bootstrap';
import { Container, Row, Col, Card } from 'react-bootstrap';
import jwt_decode from 'jwt-decode';

var decoded = jwt_decode(localStorage.getItem('jwt'));
console.log(decoded);
var userNameFromJWT = decoded.sub;
console.log(userNameFromJWT)

class UserProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
        users:[],
        tickets:[],
        isLoading: true,
        submitted: false,
        userNameJWT : '',
    };
}



componentDidMount() {
    this.getUsers();
}


getUsers() {
    const requestOptions = {
        method: 'GET',
        headers: {
            'Content-Type': 'application/json',
            'Authorization': 'Bearer ' + localStorage.getItem('jwt')
        }
    };

    this.setState({ isLoading: true });
    fetch(`${serviceConfig.baseURL}/api/users/byusername/${userNameFromJWT}`, requestOptions)
        .then(response => {
            if (!response.ok) {
                return Promise.reject(response);
                
            }
            return response.json();
        })
        .then(data => {
            if (data) {
                this.setState({ users: data, isLoading: false, tickets: data.tickets});
            }
            console.log(this.state.users);
            console.log(this.state.tickets);
            
        })
        
        .catch(response => {
            this.setState({ isLoading: false });
            NotificationManager.error(response.message || response.statusText);
            this.setState({ submitted: false });
        });
}

fillTableWithDaata(){
    console.log();
     return this.state.tickets.map(ticket => {
        return <div key = {ticket.id}>
            <ul className="text-center cursor-pointer">
     <li><b>Cinema: </b>{ticket.cinemaName}, <b>auditorium: </b> {ticket.auditoriumName}, <b>movie:</b> {ticket.movieName} <br></br> <b>seat row: </b>{ticket.seatRow} <b>seat number: </b>{ticket.seatNumber} <br></br> <b>Time: </b>{ticket.projectionTime} </li>
            </ul>
        </div>
    })
}


    render() {
        
      const users = this.state.users
      
      const rowsData = this.fillTableWithDaata();
        return (
        <Container>
        <Row className="justify-content-center">
          <Col>
            <Card className="mt-5 card-width">
              <Card.Body>
                <Card.Title><span className="card-title-font">Name: {users.firstName} {users.lastName}</span></Card.Title>
                  <hr/>
                <Card.Subtitle className="mb-2 text-muted">Username: <span className="float-right">{users.userName}</span></Card.Subtitle>
                  <hr/>
                <Card.Text>
                
                <h4>Tickets:</h4>
                      {rowsData}
                <hr/>
                </Card.Text>
                <Row className="justify-content-center font-weight-bold">
                <h5>Bonus points: {users.bonusPoints}</h5>
                </Row>
                <Row className="no-gutters pr-5 pl-5">
                </Row>
              </Card.Body>
            </Card>
          </Col>
        </Row>
      </Container>
            
        );
      }
}

export default UserProfile;