import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { Redirect } from 'react-router-dom';
import { Container, Row, Col, Card, Button, Spinner, Table} from 'react-bootstrap';
import jwt_decode from 'jwt-decode';
import { serviceConfig } from '../../appSettings';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';

var decoded = jwt_decode(localStorage.getItem('jwt'));
console.log(decoded);
var userNameFromJWT = decoded.sub;
console.log(userNameFromJWT)

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
        NotificationManager.error(response.message || response.statusText);
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
        NotificationManager.success('Successfuly payed!');
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
        <ul className="text-center cursor-pointer">
          <li><b>Cinema: </b>{ticket.cinemaName}, <b>auditorium: </b> {ticket.auditoriumName}, <b>movie:</b> {ticket.movieName} <br></br> <b>seat row: </b>{ticket.seatRow} <b>seat number: </b>{ticket.seatNumber} <br></br> <b>Time: </b>{ticket.projectionTime}
            <td width="5%" className="text-center cursor-pointer" onClick={() => this.removeTicket(ticket.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash} /></td>
                     </li>
        </ul>
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
        <Row className="justify-content-center">
          <Col>
            <h4>Unpaid tickets:</h4>
            {showTable}
          </Col>
        <Col><Button onClick={this.payment} >Pay </Button></Col>
        </Row>
      </Container>
    );
  }
}

export default Tickets;