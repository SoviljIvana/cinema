import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { Container, Col, Card, Button, Spinner, Table} from 'react-bootstrap';
import { serviceConfig } from '../../appSettings';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faTrash } from '@fortawesome/free-solid-svg-icons';
import ListGroup from 'react-bootstrap/ListGroup';


class Tickets extends Component {
  constructor(props) {
    super(props);
    this.state = {
      tickets: [],
      isLoading: true,
      submitted: false,
      userNameJWT: '',
      response1: "",
      response2: ''

    };
    this.removeTicket = this.removeTicket.bind(this);
    this.payment = this.payment.bind(this);
  }

  componentDidMount() {
    
    this.getAllUnpaidTicketsForUser();
    
  }

  getAllUnpaidTicketsForUser() {

    const token = localStorage.getItem('jwt');
    let jwtDecoder = require('jwt-decode');
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
    fetch(`${serviceConfig.baseURL}/api/tickets/allTickets/${userNameFromJWT}`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);

        }
        return response.json();
      })
      .then(data => {
        if (data) {
          this.setState({ isLoading: false, tickets: data });
        }
        console.log(this.state.tickets);

      })
      .catch(response => {
        this.setState({ isLoading: false });
        NotificationManager.error("Please log in to get tickets!");
        this.setState({ submitted: false });
      });
  }

  removeTicket(id) {
    const requestOptions = {
      method: 'DELETE',
      headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      }
  };

  fetch(`${serviceConfig.baseURL}/api/tickets/${id}`, requestOptions)
      .then(response => {
          if (!response.ok) {
              return Promise.reject(response);
          }
          
          return response.statusText;
      })
      .then(result => {
          NotificationManager.success('Successfuly removed ticket with id:', id);
          const newState = this.state.tickets.filter(ticket => {
              return ticket.id !== id;
          })
          this.setState({ tickets: newState });
      })
      .catch(response => {
          NotificationManager.error(response.message || response.statusText);
          this.setState({ submitted: false });
      });
  }

  payment() {
    const { response2 } = this.state;
    const requestOptions = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      },
    };
    fetch(`${serviceConfig.baseURL}/api/Levi9Payment`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);

        }
        console.log(response);

        return response.json();
      })
      .then(response => {
        if (response) {
          this.setState({
            response2: response.isSuccess
          });
          console.log(response);
          console.log("response2");
          console.log(this.state.response2);
          setTimeout(1500);
          this.payValue();
        }
      })
      .catch(response => {
        NotificationManager.error(response.message || response.statusText);
      });
  }

  payValue() {

    const token = localStorage.getItem('jwt');
    let jwtDecoder = require('jwt-decode');
    const decodedToken = jwtDecoder(token);
    let userNameFromJWT = decodedToken.sub;

    const { response2 } = this.state;
    const data = {
      UserName: userNameFromJWT,
      PaymentSuccess: response2

    };

    const requestOptions = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      },
      body: JSON.stringify(data)
    };
    fetch(`${serviceConfig.baseURL}/api/tickets/payValue`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(result => {
        NotificationManager.success('Successfuly paid!');
        window.open('../../UserProfile')
      })
      .catch(response => {
     
        NotificationManager.error(response.message || response.statusText);
      });
      setTimeout(3000);
      this.props.push('/userProfile');

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
        <ListGroup.Item  width="7%" className="text-center cursor-pointer" onClick={() => this.removeTicket(ticket.id)}><FontAwesomeIcon  icon={faTrash}/></ListGroup.Item>
        </ListGroup>
        <br />
      </div>
    })
  }

  render() {
    const { isLoading } = this.state;
    const rowsData = this.fillTableWithDaata();

    const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
    <tbody>
        {rowsData}
    </tbody>
      </Table>);
      const showTable = isLoading ? <Spinner/> : table;
    return (
      <Container>
        <Col>
          <Card bg="light" text="white" >
            <Card.Body>
              <Card.Text><h4 align="center" style={{color: 'black'}}>Unpaid tickets:{showTable}</h4></Card.Text>
               <Button size="lg"  variant="info" block onClick={this.payment}>Pay</Button> 
            </Card.Body>
          </Card>
        </Col>
      </Container>
    );
  }
}

export default Tickets;