import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table } from 'react-bootstrap';
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
                        <td >{movie.title}</td>
                        <td className="text-center cursor-pointer">{movie.year}</td>
                        <td className="text-center cursor-pointer">{Math.round(movie.rating)}/10</td>
                    </tr>
        })
    }
 
    render() {
        const {isLoading} = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
                            <thead>
                            <tr>
                                <th>Title</th>
                                <th className="text-center cursor-pointer">Year</th>
                                <th className="text-center cursor-pointer">Rating</th>
                            </tr>
                            </thead>
                            <tbody>
                                {rowsData}
                            </tbody>
                        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
                            
        return (
            <React.Fragment>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
      }
}

export default TopTenMovies;