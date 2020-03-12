import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../appSettings';
import { FormControl, Row, Table, FormGroup } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../Spinner';
import 'react-dropdown/style.css'
import { Typeahead } from 'react-bootstrap-typeahead';
import Select from 'react-select';
import 'bootstrap/dist/css/bootstrap.min.css';
import DateTimePicker from 'react-datetime-picker/dist/DateTimePicker';


class ProjectionsFilterForCinema extends Component {
    constructor(props) {
      super(props);
      this.state = {
          searchData: "",
          filterList: 
          [ {label: "No filter", value: 'FilterByText'},
            {label:"Filter by movie name ", value: 'moviename'},
            {label: "Filter by auditorium name ", value: 'auditname'},
            {label:"Filter by cinema name ", value :'cinemaname'},
            {label: "Filter by projection time " , value: 'dates'}],
          filter: "",
          searchString: "",
          selectedOption: "",
        startDate: '',
        endDate: '',
        projections: [],
        auditoriums: [],
        cinemas: [],
        isLoading: true,
        submitted: false,
        canSubmit: true
      };
      this.handleChange = this.handleChange.bind(this);
      this.handleSubmit = this.handleSubmit.bind(this);
      this.change = this.change.bind(this);
      this.reserveTickets = this.reserveTickets.bind(this);

    }
    
    change = selectedOption => {
    this.setState({ selectedOption });
    }

    componentDidMount() {
        this.getCinemas();
        this.getAuditoriums();

        let startDate = new Date();
        let endDate = new Date(new Date().setDate(new Date().getDate() + 1))
        let date1String = JSON.stringify(startDate);
        let date2String = JSON.stringify(endDate);
        let slicedDate1 = date1String.slice(1, -1);
        let slicedDate2= date2String.slice(1, -1);

        console.log(slicedDate1);

        this.getProjectionsInATimeSpan(slicedDate1, slicedDate2)
    }

    handleChange(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
    }

    handleSubmit(e) {
        e.preventDefault();
        this.setState({ submitted: true });
        const {searchData, selectedOption, startDate, endDate} = this.state;
        if(selectedOption.value == 'dates'){
            this.getProjectionsInATimeSpan(startDate, endDate)
            return null; 
        }
        let filter = selectedOption.value;
        if (searchData[0] == null && searchData != null)
        {
            NotificationManager.error('Please insert valid data. ');
            this.setState({ submitted: false }); 
            return null;
        }
        let searchString = searchData[0].name || searchData;
        if (searchString && filter) {
            this.getSpecificProjections(searchString, filter);
        } else {
            NotificationManager.error('Please fill in data');
            this.setState({ submitted: false });
        }
    }
    getProjectionsInATimeSpan(startDate,endDate) {
        const requestOptions = {
          method: 'GET',
          headers: {'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
        };
        let date1 = JSON.stringify(startDate);
        let date2 = JSON.stringify(endDate);

        let slicedDate1 = date1.slice(1, -1);
        let slicedDate2= date2.slice(1, -1);
        
        this.setState({isLoading: true});
        fetch(`${serviceConfig.baseURL}/api/Projections/dates/${slicedDate1},${slicedDate2}`, requestOptions)
          .then(response => {
            if (!response.ok) {
              return Promise.reject(response);
          }
          return response.json();
          })
          .then(data => {
            if (data) {
              this.setState({ projections: data.projections, isLoading: false });
              }
          })
          .catch(response => {
              this.setState({isLoading: false});
              NotificationManager.error(response.message || response.statusText);
          });
      }
    getSpecificProjections(searchString, filter) {
      const requestOptions = {
        method: 'GET',
        headers: {'Content-Type': 'application/json',
                      'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
      };
      this.setState({isLoading: true});
      fetch(`${serviceConfig.baseURL}/api/Projections/${filter}/${searchString}`, requestOptions)
        .then(response => {
          if (!response.ok) {
            return Promise.reject(response);
        }
        return response.json();
        })
        .then(data => {
          if (data) {
            this.setState({ projections: data.projections, isLoading: false });
            }
        })
        .catch(response => {
            this.setState({isLoading: false});
            NotificationManager.error(response.message || response.statusText);
        });
    }

    getAuditoriums() {
        const requestOptions = {
          method: 'GET',
          headers: {'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
        };
  
        fetch(`${serviceConfig.baseURL}/api/Auditoriums/all`, requestOptions)
          .then(response => {
            if (!response.ok) {
              return Promise.reject(response);
          }
          return response.json();
          })
          .then(data => {
            if (data) {
              this.setState({ auditoriums: data });
              }
          })
          .catch(response => {
              NotificationManager.error(response.message || response.statusText);
              this.setState({ submitted: false });
          });
      }

    getCinemas() {
        const requestOptions = {
          method: 'GET',
          headers: {'Content-Type': 'application/json',
                        'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
        };
  
        this.setState({isLoading: true});
        fetch(`${serviceConfig.baseURL}/api/Cinemas/all`, requestOptions)
          .then(response => {
            if (!response.ok) {
              return Promise.reject(response);
          }
          return response.json();
          })
          .then(data => {
            if (data) {
              this.setState({ cinemas: data, isLoading: false });
              }
          })
          .catch(response => {
              NotificationManager.error(response.message || response.statusText);
              this.setState({ isLoading: false });
          });
      }
    fillTableWithDaata() {
        return this.state.projections.map(projection => {
            return <tr key={projection.id}>
              
                        <td width="23.75%">{projection.movieTitle}</td>
                        <td width="23.75%">{projection.cinemaName}</td>
                        <td width="23.75%">{projection.auditoriumName}</td>
                        <td width="23.75%">{projection.projectionTimeString}</td>
                        <td width="5%" className="text-center cursor-pointer" onClick={() => 
                            this.reserveTickets(projection.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit}/></td>                        
                    </tr>
        })
    }
    reserveTickets(id) {
        this.props.history.push(`projectionDetails/allForProjection/${id}`);
      }



    onStartDateChange = startingDate => this.setState({ startDate: startingDate })
    onEndDateChange = endingDate => this.setState({ endDate : endingDate })

    switchFunction = (selectedOption, auditoriums, cinemas, searchData) => {
        let value = selectedOption.value;
        
        switch (value){
            case 'FilterByText':
                return <input label='Search for:'
                    id = 'searchData'
                    type = 'text'
                    value = {searchData = null}
                    placeholder = "Insert search data"
                    onChange = {this.handleChange}
                    />
            case 'moviename':
                return <input 
                    id = 'searchData'
                    type = 'text'
                    value = {searchData = null}
                    placeholder = "Insert search data"
                    onChange = {this.handleChange}
                    />
            case 'auditname':
                return <Typeahead 
                labelKey="name"
                 id = 'auditorium'
                 options = {auditoriums}
                 value = {searchData}
                 onChange = {searchData => this.setState({ searchData })}
                 />
            case 'cinemaname':
                return <Typeahead 
                labelKey="name"
                 id = 'cinema'
                 options = {cinemas}
                 value = {searchData}
                 onChange = {searchData => this.setState({ searchData })}
                 />
            case 'dates': 
                return [<DateTimePicker
                            value = {this.state.startDate}
                            onChange = {this.onStartDateChange}                 
                        />,
                       <DateTimePicker
                            value = {this.state.endDate}
                            onChange = {this.onEndDateChange}
                       />]

        }
    }

    render() {
        const {isLoading, searchData, auditoriums, cinemas, filterList, selectedOption} = this.state;
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
                <FormGroup class = "set-overflow-y:auto">
                        <Select
                            id = 'filterOptions'
                            options= {filterList}
                            value = {selectedOption}
                            onChange={this.change}
                            placeholder="Choose filter"
                        />
                        <div>{this.switchFunction(selectedOption, auditoriums, cinemas, searchData)}</div>
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

export default ProjectionsFilterForCinema;