import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { Table } from 'react-bootstrap';
import { Container, Row, Col, Card } from 'react-bootstrap';
import Spinner from '../components/Spinner'
import ListGroup from 'react-bootstrap/ListGroup';


class UserProfile extends Component {
  constructor(props) {
    super(props);
    this.state = {
      users: [],
      tickets: [],
      submitted: false,
      userNameJWT: '',
      isLoading: true,
    };
  }

  componentDidMount() {
    this.getUsers();
  }

  getUsers() {
        const token = localStorage.getItem('jwt');
        var jwtDecoder = require('jwt-decode');
        const decodedToken = jwtDecoder(token);
        let userNameFromJWT = decodedToken.sub;

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
          this.setState({ users: data, isLoading: false, tickets: data.tickets });
        }
        console.log(this.state.users);
        console.log(this.state.tickets);
      })
      .catch(response => {
        this.setState({ isLoading: false });
        NotificationManager.error("You are not logged in! Please log in to view your profile!");
        this.setState({ submitted: false });
      });
  }

  fillTableWithDaata() {

    return this.state.tickets.map(ticket => {
      return <div key={ticket.id}>
        <br></br>
        <ListGroup variant="flush">
        <ListGroup.Item variant="info"> TITLE - {ticket.movieName} </ListGroup.Item>
        <ListGroup.Item variant="info"> TIME - {ticket.projectionTime} </ListGroup.Item>
        <ListGroup.Item STYLE="text-transform:uppercase" variant="info">  {ticket.auditoriumName} / row {ticket.seatRow}, number {ticket.seatNumber} </ListGroup.Item >
          <ListGroup.Item variant="info" > CINEMA- {ticket.cinemaName}</ListGroup.Item>
        </ListGroup>
        <br />
      </div>
    })
  }

  render() {
    const { isLoading } = this.state;
    const rowsData = this.fillTableWithDaata();
    const users = this.state.users
    const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
      <tbody>
        {rowsData}
      </tbody>
    </Table>);
    const showTable = isLoading ? <Spinner /> : table;
    return (
      <Container>
        <Col>
          <Card bg="light" text="white" >
            <Card.Body>
              <Card.Title > <h3 align="left" style={{color: 'black'}}>Name: {users.firstName} {users.lastName}</h3></Card.Title>
              <Card.Subtitle><h3  align="left" style={{color: 'black'}}>Username: {users.userName}</h3></Card.Subtitle>
              <Card.Text><h3 align="left" style={{color: 'black'}}>Bonus points: {users.bonusPoints}</h3></Card.Text>
              <Card.Text><h4 align="center" style={{color: 'black'}}>Tickets:{showTable}</h4></Card.Text>
            </Card.Body>
          </Card>
        </Col>
      </Container>
    );
  }
}
export default UserProfile;