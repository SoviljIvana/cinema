import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { Container, Col, Card, Button,ButtonToolbar, ListGroupItem, CardBody, CardDeck } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { Row, Table } from 'react-bootstrap';
import Spinner from '../Spinner';
import ReactStars from 'react-stars';
import ListGroup from 'react-bootstrap/ListGroup';
import 'bootstrap/dist/css/bootstrap.min.css';
import './App.css';
import { Fade } from 'react-slideshow-image';
import Image2 from './movie2.jpg';
import Image3 from './movie3.png';
import './App.css';
import { Link } from 'react-router-dom';

const fadeImages = [
  Image2,
  Image3,
];
const fadeProperties = {
  duration: 5000,
  transitionDuration: 500,
  infinite: false,
  indicators: true,
  onChange: (oldIndex, newIndex) => {
    console.log(`fade transition from ${oldIndex} to ${newIndex}`);
  }
}
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
      auditoriumName: '',
      searchData: ''

    };
    //    this.details = this.details.bind(this);
    this.seatsForProjection = this.seatsForProjection.bind(this);
    this.renderProjectionButtons = this.renderProjectionButtons.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleTopTen = this.handleTopTen.bind(this);
    this.handleShowAll = this.handleShowAll.bind(this);

    //this.fillTableWithDaata = this.fillTableWithDaata.bind(this);
  }

  handleChange(e) {
    const { id, value } = e.target;
    this.setState({ [id]: value });
  }

  handleSubmit(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    const { searchData } = this.state;
    if (searchData) {
      this.getSearch(searchData);
    } else {
      NotificationManager.error('Please fill in data');
      this.setState({ submitted: false });
    }
  }
  handleTopTen(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    this.getTopTenMovies();
  }

  handleShowAll(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    this.getMovies();
  }

  componentDidMount() {
    this.getMovies();

  }

  getSearch(searchData) {
    const requestOptions = {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      }
    };

    this.setState({ isLoading: true });
    fetch(`${serviceConfig.baseURL}/api/Movies/search/${searchData}`, requestOptions)
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
        NotificationManager.error("No results, please try again. ");
        this.setState({ submitted: false });
      });
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

  getTopTenMovies() {
    const requestOptions = {
      method: 'GET',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      }
    };
    this.setState({ isLoading: true });
    fetch(`${serviceConfig.baseURL}/api/Movies/top`, requestOptions)
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
    this.props.history.push(`projectionDetails/allForProjection/` + `${id}`);
  }

  renderProjectionButtons(listOfProjections) {

    return listOfProjections.map((list) => {
      return <ListGroup.Item key={list.id}><Button variant="dark" onClick={() => this.seatsForProjection(list.id)}>{list.projectionTimeString}</Button></ListGroup.Item>
    })
  }

  fillTableWithDaata() {

    return this.state.movies.map(movie => {
      return <div key={movie.id} >
        <br></br>
        <div class="movie" style={{ width: '77rem' }} key={movie.id}>
          <Card.Header class="sub-section-title" as="h5">{movie.title}</Card.Header>
          <ListGroup class="movie-details" variant="flush">
          <ListGroup.Item > Year: {movie.year}</ListGroup.Item>
          <ListGroup.Item > Rating:<ReactStars count={10} onChange={ratingChanged} edit={false} size={37} value={movie.rating} color1={'black'} color2={'#ffd700'} /></ListGroup.Item >
          <ListGroup.Item > Projections:{this.renderProjectionButtons(movie.listOfProjections)}</ListGroup.Item>
          </ListGroup>
        </div>
        <br />
      </div>
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
    const { isLoading, searchData } = this.state;
    const rowsData = this.fillTableWithDaata();
    const table = (<table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
      <tbody>
        {rowsData}
      </tbody>
    </table>);
    const showTable = isLoading ? <Spinner></Spinner> : table;
    return (

      <Row className="no-gutters pr-5 pl-5">
        <p className="slide-container">
          <Fade {...fadeProperties}>
            <div className="each-fade">
              <img src={fadeImages[0]} />
            </div>
            <div className="each-fade">
              <img src={fadeImages[1]} />
            </div>
           
          </Fade>
        </p>
        <p>
          <Button onClick={this.handleShowAll}>Show all</Button>
        </p>
        <p>
          <button onClick={this.handleTopTen}>Show Top 10</button>
        </p>
          <p>
            <label for='searchData'>Search for a movie by tags OR movie title:</label>
            <input
              id='searchData'
              type='text'
              value={searchData}
              placeholder="Insert search data"
              onChange={this.handleChange}
            />
            <Button onClick={this.handleSubmit}>Search</Button>
            <Link className="text-decoration-none" to='/ProjectionsFilterForCinema'><Button >Filter projections</Button></Link>
            <br></br>
             </p>
       
    <div>
          {showTable}</div>
      </Row>
    );
  }
}
export default AllProjectionsForCinema;