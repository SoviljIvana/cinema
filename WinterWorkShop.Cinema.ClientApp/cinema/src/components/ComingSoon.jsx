import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { Container, Row, Col, Card } from 'react-bootstrap';
import jwt_decode from 'jwt-decode';
import ListGroup from 'react-bootstrap/ListGroup';
import ReactStars from 'react-stars';
import Spinner from '../components/Spinner'

const ratingChanged = (newRating) => {
    console.log(newRating)
  }
var decoded = jwt_decode(localStorage.getItem('jwt'));
console.log(decoded);
var userNameFromJWT = decoded.sub;
console.log(userNameFromJWT)

class ComingSoon extends Component {
    constructor(props) {
        super(props);
        this.state = {
          movies: [],
          submitted: false,
          isLoading: true,
          searchData: '',

        };
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
    fetch(`${serviceConfig.baseURL}/api/Movies/comingsoon`, requestOptions)
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

  fillTableWithDaata(){
    return this.state.movies.map(movie => {
        return <div key={movie.id} >
          <br></br>
          <div class="movie" style={{ width: '120rem' }} key={movie.id}>
            <Card.Header class="sub-section-title" as="h5">{movie.title}</Card.Header>
            <ListGroup class="movie-details" variant="flush">
              <ListGroup.Item> Year: {movie.year}</ListGroup.Item>
              <ListGroup.Item> Rating:<ReactStars count={10} onChange={ratingChanged} edit={false} size={37} value={movie.rating} color1={'black'} color2={'#ffd700'} /></ListGroup.Item >
              <ListGroup.Item><b>Genres:</b> {movie.tagsMovieModel.generes}<br></br><b>Directories:</b> {movie.tagsMovieModel.directores}<br></br><b> Duration: </b>{movie.tagsMovieModel.duration} min <br></br><b> Awards:</b> {movie.tagsMovieModel.awards}
    
    <br></br> <b>Languages:</b>{movie.tagsMovieModel.languages}<br></br> <b>States:</b> {movie.tagsMovieModel.states}<br></br> <b>Actores:</b> {movie.tagsMovieModel.actores}
     </ListGroup.Item>
           </ListGroup>
         

          </div>
          <br />
        </div>
      })
  }

  render() {
    const { isLoading } = this.state;
    const rowsData = this.fillTableWithDaata();
    const table = (<table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
      <tbody>
        {rowsData}
      </tbody>
    </table>);
    const showTable = isLoading ? <Spinner></Spinner> : table;
    return (
        <Row>
          {showTable}
        </Row>
    );
}}
export default ComingSoon;