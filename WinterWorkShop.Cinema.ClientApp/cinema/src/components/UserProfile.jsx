import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { Table } from 'react-bootstrap';
import { Container, Row, Col, Card } from 'react-bootstrap';
import jwt_decode from 'jwt-decode';
import Spinner from '../components/Spinner'
import ListGroup from 'react-bootstrap/ListGroup';

var decoded = jwt_decode(localStorage.getItem('jwt'));
console.log(decoded);
var userNameFromJWT = decoded.sub;
console.log(userNameFromJWT)

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
        NotificationManager.error(response.message || response.statusText);
        this.setState({ submitted: false });
      });
  }

  fillTableWithDaata() {
    console.log();
    return this.state.tickets.map(ticket => {
      return <div key={ticket.id}>
        <br></br>
        <ListGroup variant="flush">
          <ListGroup.Item variant="primary" > Cinema:     {ticket.cinemaName}</ListGroup.Item>
          <ListGroup.Item variant="secondary"> Auditorium: {ticket.auditoriumName} </ListGroup.Item >
          <ListGroup.Item variant="success"> Movie:      {ticket.movieName} </ListGroup.Item>
          <ListGroup.Item variant="danger"> Seat row:   {ticket.seatRow}</ListGroup.Item>
          <ListGroup.Item variant="warning"> Seat number:{ticket.seatNumber}</ListGroup.Item>
          <ListGroup.Item variant="info"> Time:       {ticket.projectionTime} </ListGroup.Item>
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
                <Card.Title><span className="card-title-font">Name: {users.firstName} {users.lastName}</span></Card.Title>
                <Card.Subtitle className="mb-2 text-muted">Username: <span className="float-right">{users.userName}</span></Card.Subtitle>
                <Card.Text><h3>Tickets:{showTable}</h3></Card.Text>
                <Card.Text>Bonus points:{users.bonusPoints}</Card.Text>
              </Card.Body>
            </Card>
          </Col>
      </Container>
    );
  }
}
export default UserProfile;