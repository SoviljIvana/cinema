import React, { Component } from 'react';
import { Container, Card, Button, Form, FormControl, Nav, Navbar, Row } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
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
import { MDBCol, MDBBtn, MDBFormInline } from "mdbreact";
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
      movies: [],
      isLoading: true,
      searchData: ''
    };
    this.seatsForProjection = this.seatsForProjection.bind(this);
    this.renderProjectionButtons = this.renderProjectionButtons.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleTopTen = this.handleTopTen.bind(this);
    this.handleShowAll = this.handleShowAll.bind(this);
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
        <div class="movie" style={{ width: '100rem' }} key={movie.id}>
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
        <Navbar className="slide-container"  expand="lg" variant="light" bg="light">
          <Nav justify variant="tabs" className="mr-auto">
            <Container>
              <Navbar.Brand><Button size= "lg" variant="outline-dark" onClick={this.handleShowAll}>Show all movies</Button> </Navbar.Brand>
              <Navbar.Brand><Button  size= "lg" variant="outline-dark" onClick={this.handleTopTen}>Show Top 10 movies</Button> </Navbar.Brand>
              <Navbar.Brand><Link to='/ProjectionsFilterForCinema'><Button  size= "lg" variant="outline-dark">Filter projections</Button></Link></Navbar.Brand>
           
            <Navbar.Toggle className="text-white" />
          <Navbar.Collapse id="basic-navbar-nav" className="text-white">
          </Navbar.Collapse>
          </Container>
          </Nav>
            
            <Form inline onSubmit={this.handleSubmit}>
              <FormControl  size= "lg"
                type="text" className="form-control mr-sm-2" placeholder="Search movie" aria-label="Search" for='searchData'
                className="mr-sm-2"
                id='searchData'
                type='text'
                value={searchData}
                onChange={this.handleChange} />
              <Button  size= "lg" type="submit" variant="outline-dark" className="mr-1">Search</Button>
            </Form>
          
           
         
         
        </Navbar>
        <div>
          {showTable}
        </div>
      </Row>
    );
  }
}
export default AllProjectionsForCinema;