import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table ,Container, Button} from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';

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
        fetch(`${serviceConfig.baseURL}/api/Movies/allMovies`, requestOptions)
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
                <td >{movie.title}</td>
                <td className="text-center cursor-pointer">{movie.year}</td>
                <td className="text-center cursor-pointer">{Math.round(movie.rating)}/10</td>
                <td  width="1%"  className="text-center cursor-pointer">{movie.current ? 'Yes' : 'No'}</td> <td className="text-center cursor-pointer" onClick={() => this.editMovie(movie.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit} /></td>
                <td  width="1%"  className="text-center cursor-pointer" onClick={() => this.removeMovie(movie.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash} /></td>
            </tr>
        })
    }

    editMovie(id) {
        this.props.history.push(`editmovie/${id}`);
    }

    render() {
        const { isLoading, searchData } = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
            <thead>
                <tr>
                    <th >Title</th>
                    <th className="text-center cursor-pointer">Year</th>
                    <th className="text-center cursor-pointer">Rating</th>
                    <th className="text-center cursor-pointer">Current</th>
                    <th className="text-center cursor-pointer">Edit</th>
                    <th className="text-center cursor-pointer">Delete</th>
                </tr>
            </thead>
            <tbody>
                {rowsData}
            </tbody>
        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
        return (
            <React.Fragment>
                <Container>
                <div for = 'searchData'>
               <h4> <b>Search for a movie by tags OR movie title: </b></h4>
                <br></br>
  
                <input
                    id = 'searchData'
                    type = 'text'
                    value = {searchData}
                    placeholder = "Insert search data"
                    onChange = {this.handleChange}
                    />
                    
                 <Button variant="info" onClick = {this.handleSubmit}>Search</Button>  </div>
            <br></br>
            <br></br>
            <Row className="no-gutters pr-5 pl-5">
             {showTable}
            </Row>
            </Container>
            </React.Fragment> 
        );
    }
}

export default ShowAllMovies;