import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';
import Switch from "react-switch";
import ReactStars from 'react-stars';

const ratingChanged = (newRating) => {
    console.log(newRating)
}

class TopTenMovies extends Component {

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
        headers: {'Content-Type': 'application/json',
                      'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
      };

      this.setState({isLoading: true});
      fetch(`${serviceConfig.baseURL}/api/Movies/top`, requestOptions)
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
            this.setState({isLoading: false});
            NotificationManager.error(response.message || response.statusText);
            this.setState({ submitted: false });
        });
    }

    fillTableWithDaata() {
        return this.state.movies.map(movie => {
            return <tr key={movie.id}>
                        <td className="text-center cursor-pointer">{movie.title}</td>
                        <td className="text-center cursor-pointer">{movie.year}</td>
                        <td className="text-center cursor-pointer">{<ReactStars count={10} onChange={ratingChanged} edit = {false} size={37} value={movie.rating} color1 = {'grey'} color2={'#ffd700'} />}</td>
                        <td className="text-center cursor-pointer">{movie.current ? <Switch onChange={this.handleChange} checked =  {true} /> : <Switch onChange={this.handleChange} checked =  {false} />} </td>
                    </tr>
        })
    }
 
    render() {
        const {isLoading} = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table striped bordered hover size="sm">
                            <thead>
                            <tr>
                                <th className="text-center cursor-pointer">Title</th>
                                <th className="text-center cursor-pointer">Year</th>
                                <th className="text-center cursor-pointer">Rating</th>
                                <th className="text-center cursor-pointer">Is Current</th>
                                <th className="text-center cursor-pointer"></th>
                            </tr>
                            </thead>
                            <tbody>
                                {rowsData}
                            </tbody>
                        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
                            
        return (
            <React.Fragment>
                <Row className="no-gutters pt-2">
                    <h1 className="form-header ml-2">Top 10 Movies</h1>
                </Row>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
      }
}

export default TopTenMovies;