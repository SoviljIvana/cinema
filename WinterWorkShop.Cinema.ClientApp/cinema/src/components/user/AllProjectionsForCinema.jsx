import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { Container, Col, Card, Button, ListGroupItem } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Table } from 'react-bootstrap';
import Spinner from '../Spinner';
import Switch from "react-switch";
import ReactStars from 'react-stars';
import ListGroup from 'react-bootstrap/ListGroup';
import 'bootstrap/dist/css/bootstrap.min.css';

const ratingChanged = (newRating) => {
  console.log(newRating)
}
class AllProjectionsForCinema extends Component {
  constructor(props) {
    super(props);
    this.state = {
      movies: [],
      isLoading: true
    };
  }

  componentDidMount() {
    this.getProjections();
  }

  getProjections() {
    const requestOptions = {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      }
    };

    this.setState({ isLoading: true });
    fetch(`${serviceConfig.baseURL}/api/Movies/current`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        if (data) {
          this.setState({ movies: data, isLoading: false });
        }
      })
      .catch(response => {
        this.setState({ isLoading: false });
        NotificationManager.error(response.message || response.statusText);
        this.setState({ submitted: false });
      });

  }

  fillTableWithDaata() {
    return this.state.movies.map(movie => {
      return  <>
       <Card bg="light" style={{ width: '100rem' }} key={movie.id}>
        <Card.Header text="white" style={{ width: '100rem' }}>{movie.title}</Card.Header>
        <ListGroup className="list-group-flush">
          <ListGroupItem text="white" style={{ width: '100rem' }}>{movie.year}</ListGroupItem>
          <ListGroupItem bg="dark" text="white" style={{ width: '100rem' }}>{<ReactStars count={10} onChange={ratingChanged} edit={false} size={37} value={movie.rating} color1={'grey'} color2={'#ffd700'} />}</ListGroupItem>
        </ListGroup>
      </Card>
      <br />
      </>

    })
  }

  render() {
    const { isLoading } = this.state;
    const rowsData = this.fillTableWithDaata();
    const table = (<Table striped bordered hover size="sm">
      <tbody>
        {rowsData}
      </tbody>
    </Table>);
    const showTable = isLoading ? <Spinner></Spinner> : table;
    return (
      <React.Fragment>
        <Row className="no-gutters pr-5 pl-5">
          <br>
          </br>
          {showTable}
          <br>
          </br>
        </Row>
      </React.Fragment>
    );
  }
}

export default withRouter(AllProjectionsForCinema);