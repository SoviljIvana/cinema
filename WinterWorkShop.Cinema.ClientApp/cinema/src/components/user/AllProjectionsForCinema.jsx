import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { Container, Col, Card, Button, ListGroupItem } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Table } from 'react-bootstrap';
import Spinner from '../Spinner';
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
      projections: [],
      movies: [],
      isLoading: true,
      projectionTime: '',
      movieId: '',
      auditoriumId: '',
      auditoriumName: ''
    };
//    this.details = this.details.bind(this);
    this.seatsForProjection = this.seatsForProjection.bind(this);
    this.renderProjectionButtons = this.renderProjectionButtons.bind(this);
    //this.fillTableWithDaata = this.fillTableWithDaata.bind(this);
  }

  componentDidMount() {
    this.getMovies();

  }

  getMovies() {
    const requestOptions = {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      }
    };
    this.setState({ isLoading: true });
    fetch(`${serviceConfig.baseURL}/api/Movies/currentMoviesWithProjections`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        if (data) {
          this.setState({
            movies: data,
            isLoading: false
          });
        }

      })
      .catch(response => {
        this.setState({ isLoading: false });
        NotificationManager.error(response.message || response.statusText);
        this.setState({ submitted: false });
      });

  }
 
  seatsForProjection(id) {
    this.props.history.push(`projectionDetails/allForProjection/`+ `${id}`);
  }

  renderProjectionButtons(listOfProjections) {

    return listOfProjections.map((list) => {
      return <ul key={list.id}>
        <br></br>
        <Button onClick={() => this.seatsForProjection(list.id)}>{list.projectionTimeString}</Button>
        <br />
      </ul>
    })
  }

  fillTableWithDaata() {

    return this.state.movies.map(movie => {
      return <ul key={movie.id}>
        <br></br>
        <Card border="primary" style={{ width: '100rem' }} key={movie.id}>
          <Card.Header text="white" style={{ width: '99rem' }}><Button variant="link" className="text-center cursor-pointer" onClick={() => this.details(movie.id)}>{movie.title}</Button></Card.Header>
          <ListGroup className="list-group-flush">
            <ListGroupItem text="white" style={{ width: '99rem' }}>{movie.year}</ListGroupItem>
            <ListGroupItem bg="dark" text="white" style={{ width: '99rem' }}>{<ReactStars count={10} onChange={ratingChanged} edit={false} size={37} value={movie.rating} color1={'grey'} color2={'#ffd700'} />}</ListGroupItem>
            <ListGroupItem >{this.renderProjectionButtons(movie.listOfProjections)} </ListGroupItem>
          </ListGroup>
        </Card>
        <br />
      </ul>
    })
  }

  // details(id) {
  //   this.props.history.push(`projectiondetails/${id}`);
  // }

  renderRows(rows, seats) {
    const rowsRendered = [];
    for (let i = 0; i < rows; i++) {
      rowsRendered.push(<tr key={i}> {this.renderSeats(seats, i)}</tr>);
    }
    return rowsRendered;
  }

  renderSeats(seats, row) {
    let renderedSeats = [];
    for (let i = 0; i < seats; i++) {
      renderedSeats.push(<button key={'row: ' + row + ', seat: ' + i}>{row}, {i}</button>);
    }
    return renderedSeats;
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

export default AllProjectionsForCinema;