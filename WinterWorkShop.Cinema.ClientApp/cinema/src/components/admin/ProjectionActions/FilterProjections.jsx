import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { FormControl, Row, Table, FormGroup, Button } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';
import 'react-dropdown/style.css'
import { Typeahead } from 'react-bootstrap-typeahead';
import Select from 'react-select';
import 'bootstrap/dist/css/bootstrap.min.css';
import DateTimePicker from 'react-datetime-picker/dist/DateTimePicker';


class FilterProjections extends Component {
    constructor(props) {
        super(props);
        this.state = {
            searchData: "",
            filterList:
                [{ label: "No filter", value: 'FilterByText' },
                { label: "Filter by movie name ", value: 'moviename' },
                { label: "Filter by auditorium name ", value: 'auditname' },
                { label: "Filter by cinema name ", value: 'cinemaname' },
                { label: "Filter by projection time ", value: 'dates' }],
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
        this.editProjection = this.editProjection.bind(this);
        this.removeProjection = this.removeProjection.bind(this);
        this.handleChange = this.handleChange.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.change = this.change.bind(this);
    }

    change = selectedOption => {
        this.setState({ selectedOption });
    }

    componentDidMount() {
        this.getCinemas();
        this.getAuditoriums();
        let startingString = "qwertyuiopasdfghjkl"
        let startingFilter = "FilterByText"
        //this.getSpecificProjections(startingString, startingFilter);

    }

    handleChange(e) {
        const { id, value } = e.target;
        this.setState({ [id]: value });
    }

    handleSubmit(e) {
        e.preventDefault();
        this.setState({ submitted: true });
        const { searchData, selectedOption, startDate, endDate } = this.state;
        if (selectedOption.value == 'dates') {
            this.getProjectionsInATimeSpan(startDate, endDate)
            return null;
        }
        let filter = selectedOption.value;
        if (searchData[0] == null && searchData != null) {
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
    getProjectionsInATimeSpan(startDate, endDate) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };
        let date1 = JSON.stringify(startDate);
        let date2 = JSON.stringify(endDate);

        let slicedDate1 = date1.slice(1, -1);
        let slicedDate2 = date2.slice(1, -1);

        this.setState({ isLoading: true });
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
                this.setState({ isLoading: false });
                NotificationManager.error("No results, please try again!");
            });
    }
    getSpecificProjections(searchString, filter) {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };
        this.setState({ isLoading: true });
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
                this.setState({ isLoading: false });
                NotificationManager.error("No results, please try again!");
            });
    }

    getAuditoriums() {
        const requestOptions = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
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
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        this.setState({ isLoading: true });
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

    removeProjection(id) {
        const requestOptions = {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/projections/${id}`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.statusText;
            })
            .then(result => {
                NotificationManager.success('Successfuly removed projection with id:', id);
                const newState = this.state.projections.filter(projection => {
                    return projection.projection.id !== id;
                })
                this.setState({ projections: newState });
            })
            .catch(response => {
                NotificationManager.error("Unable to delete projection.");
                this.setState({ submitted: false });
            });
    }

    fillTableWithDaata() {
        return this.state.projections.map(projection => {
            return <tr key={projection.id}>
                <td >{projection.movieTitle}</td>
                <td className="text-center cursor-pointer">{projection.cinemaName}</td>
                <td className="text-center cursor-pointer">{projection.auditoriumName}</td>
                <td className="text-center cursor-pointer" >{projection.projectionTimeString}</td>
                <td className="text-center cursor-pointer" onClick={() => this.removeProjection(projection.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash} /></td>
            </tr>
        })
    }

    editProjection(id) {
        this.props.history.push(`editProjection/${id}`);
    }
    onStartDateChange = startingDate => this.setState({ startDate: startingDate })
    onEndDateChange = endingDate => this.setState({ endDate: endingDate })
    switchFunction = (selectedOption, auditoriums, cinemas, searchData) => {
        let value = selectedOption.value;
        switch (value) {
            case 'FilterByText':
                return <input label='Search for:'
                    id='searchData'
                    type='text'
                    value={searchData = null}
                    placeholder="Insert search data"
                    onChange={this.handleChange}
                />
            case 'moviename':
                return <input
                    id='searchData'
                    type='text'
                    value={searchData = null}
                    placeholder="Insert search data"
                    onChange={this.handleChange}
                />
            case 'auditname':
                return <Typeahead
                    labelKey="name"
                    id='auditorium'
                    options={auditoriums}
                    value={searchData}
                    onChange={searchData => this.setState({ searchData })}
                />
            case 'cinemaname':
                return <Typeahead
                    labelKey="name"
                    id='cinema'
                    options={cinemas}
                    value={searchData}
                    onChange={searchData => this.setState({ searchData })}
                />
            case 'dates':
                return [<DateTimePicker
                    value={this.state.startDate}
                    onChange={this.onStartDateChange}
                />,
                <DateTimePicker
                    value={this.state.endDate}
                    onChange={this.onEndDateChange}
                />]
        }
    }

    render() {
        const { isLoading, searchData, auditoriums, cinemas, filterList, selectedOption } = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
            <thead>
                <tr>
                    <th>Movie Title</th>
                    <th className="text-center cursor-pointer">Cinema Name</th>
                    <th className="text-center cursor-pointer">Auditorium Name</th>
                    <th className="text-center cursor-pointer">Projection Time</th>
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
                <FormGroup inline onSubmit={this.handleSubmit}>
                    <Select
                        type="text" className="form-control mr-sm-2" placeholder="Search movie" aria-label="Search" for='searchData'
                        className="mr-sm-2"
                        id='filterOptions'
                        options={filterList}
                        value={selectedOption}
                        onChange={this.change}
                        placeholder="Choose filter"
                    />
                    <br></br>
                    <div>{this.switchFunction(selectedOption, auditoriums, cinemas, searchData)}</div>
                    <br></br>
                    <Button  size="lg" type="submit" variant="secondary" onClick={this.handleSubmit}>Search</Button>
                    <br></br>

            </FormGroup>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
    }
}

export default FilterProjections;