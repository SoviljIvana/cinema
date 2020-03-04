import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { FormControl, Row, Table, FormGroup } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';
import Dropdown from 'react-dropdown'
import 'react-dropdown/style.css'
import { Typeahead } from 'react-bootstrap-typeahead';
import Select from 'react-select';
import 'bootstrap/dist/css/bootstrap.min.css';





class FilterProjections extends Component {
    constructor(props) {
      super(props);
      this.state = {
          searchData: "",
          filterList: 
          [ {label: "No filter", value: 'FilterByText'},
            {label:"Filter by movie name ", value: 'moviename'},
            {label:"Filter by cinema name ", value :'cinemaname'},
            {label: "Filter by auditorium name ", value: 'auditname'},
            {label: "Filter by projection time " , value: 'dates'}],
          filter: "",
          alll:"",
          selectedOption: "",
        projections: [],
        isLoading: true,
        submitted: false,
        canSubmit: true
      };
      this.editProjection = this.editProjection.bind(this);
      this.removeProjection = this.removeProjection.bind(this);
      this.handleChange = this.handleChange.bind(this);
      this.handleSubmit = this.handleSubmit.bind(this);
      this.change = this.change.bind(this);

    }
    change = selectedOption => {
    this.setState({ selectedOption });
    return <input>hahahha</input>
}

    componentDidMount() {
        let startingString = "qwertyuiopasdfghjkl"
        let startingFilter ="FilterByText"
      this.getProjections(startingString, startingFilter);
      
    }

    handleChange(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
    }

    handleSubmit(e) {
        e.preventDefault();
        this.setState({ submitted: true });
        const {searchData, selectedOption} = this.state;
        let filter = selectedOption.value
        if (searchData && filter) {
            this.getProjections(searchData, filter);
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }

    getProjections(searchData, filter) {
      const requestOptions = {
        method: 'GET',
        headers: {'Content-Type': 'application/json',
                      'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
      };
      this.setState({isLoading: true});
      fetch(`${serviceConfig.baseURL}/api/Projections/${filter}/${searchData}`, requestOptions)
        .then(response => {
          if (!response.ok) {
            return Promise.reject(response);
        }
        return response.json();
        })
        .then(data => {
          if (data) {
            this.setState({ projections: data, isLoading: false });
            }
        })
        .catch(response => {
            this.setState({isLoading: false});
            NotificationManager.error(response.message || response.statusText);
        });
    }

    removeProjection(id) {
        // to be implemented
    }

    fillTableWithDaata() {
        return this.state.projections.map(projection => {
            return <tr key={projection.id}>
              
                        <td width="22.5%">{projection.projection.movieTitle}</td>
                        <td width="22.5%">{projection.projection.cinemaName}</td>
                        <td width="22.5%">{projection.projection.aditoriumName}</td>
                        <td width="22.5%">{projection.projection.projectionTime}</td>
                        <td width="5%" className="text-center cursor-pointer" onClick={() => this.editProjection(projection.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit}/></td>
                        <td width="5%" className="text-center cursor-pointer" onClick={() => this.removeProjection(projection.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash}/></td>
                        
                    </tr>
        })
    }

    editProjection(id) {
        // to be implemented
        this.props.history.push(`editProjection/${id}`);
    }

    render() {
        const {isLoading, searchData, alll, filterList, selectedOption} = this.state;
        let{filter} = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table striped bordered hover size="sm" variant="dark">
                            <thead>
                            <tr>
                                <th>Movie Title</th>
                                <th>Cinema Name</th>
                                <th>Auditorium Name</th>
                                <th>Projection Time</th>
                            </tr>
                            </thead>
                            <tbody>
                                {rowsData}
                            </tbody>
                        </Table>);
        const showTable = isLoading ? <Spinner></Spinner> : table;
        return (
            <React.Fragment>
                <FormGroup>
                        <label for='searchData'>Enter search parameter: </label>
                        <input 
                            id = 'searchData'
                            type = 'text'
                            value = {searchData}
                            onChange = {this.handleChange}
                            />
                        <Select
                            id = 'filterOptions'
                            options= {filterList}
                            value = {selectedOption}
                            onChange={this.change}
                            placeholder="Choose filter"
                        />
                        <button onClick = {this.handleSubmit}>Confirm</button>           
                </FormGroup>
                <Row className="no-gutters pt-2">
                    <h1 className="form-header ml-2">Search Results</h1>
                </Row>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
      }
}

export default FilterProjections;