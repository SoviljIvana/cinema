import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { Container, Row, Col, Card } from 'react-bootstrap';
import jwt_decode from 'jwt-decode';

class Tickets extends Component {
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
    render() {
        return (
        <Container>
        <Row className="justify-content-center">
          <Col>
             <h4>Tickets:</h4>         
          </Col>
        </Row>
      </Container>
        );
      }
}

export default Tickets;