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

class ShowAllMovies extends Component {

    constructor(props) {
        super(props);
        this.state = {
            searchData:"",
            movies: [],
            isLoading: true,

        };
        this.editMovie = this.editMovie.bind(this);
        this.removeMovie = this.removeMovie.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
    }

    handleChange(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
    }

    handleSubmit(e){
        e.preventDefault(); 
        this.setState({submitted: true});
        const {searchData} = this.state;
        if(searchData){
            this.getSearch(searchData);
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }

    componentDidMount() {
        this.getProjections();
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

    getProjections() {
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
                    this.setState({ movies: data, isLoading: false });
                }
            })
            .catch(response => {
                this.setState({ isLoading: false });
                NotificationManager.error("response.message || response.statusText");
                this.setState({ submitted: false });
            });
    }

    removeMovie(id) {
        const requestOptions = {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/movies/${id}`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.statusText;
            })
            .then(result => {
                NotificationManager.success('Successfuly removed movie with id:', id);
                const newState = this.state.movies.filter(movie => {
                    return movie.id !== id;
                })
                this.setState({ movies: newState });
            })
            .catch(response => {
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
                <td className="text-center cursor-pointer">{movie.current ? <Switch onChange={this.handleChange} checked={true} /> : <Switch onChange={this.handleChange} checked={false} />} </td>
                <td className="text-center cursor-pointer" onClick={() => this.editMovie(movie.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit} /></td>
                <td className="text-center cursor-pointer" onClick={() => this.removeMovie(movie.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash} /></td>
           
           
            </tr>
        })
    }

    editMovie(id) {
        this.props.history.push(`editmovie/${id}`);
    }

    render() {
        const { isLoading, searchData } = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table striped bordered hover size="sm">
            <thead>
                <tr>
                    <th className="text-center cursor-pointer">Title</th>
                    <th className="text-center cursor-pointer">Year</th>
                    <th className="text-center cursor-pointer">Rating</th>
                    <th className="text-center cursor-pointer">Is Current</th>
                </tr>
            </thead>
            <tbody>
                {rowsData}
            </tbody>
        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
        return (
            <React.Fragment>
                <label for = 'searchData'>Search for a movie by tags OR movie title:</label>
                <input
                    id = 'searchData'
                    type = 'text'
                    value = {searchData}
                    placeholder = "Insert search data"
                    onChange = {this.handleChange}
                    />
                <button onClick = {this.handleSubmit}>Confirm</button>   
                <Row className="no-gutters pt-2">
                    <h1 className="form-header ml-2">All Movies</h1>
                </Row>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
    }
}

export default ShowAllMovies;