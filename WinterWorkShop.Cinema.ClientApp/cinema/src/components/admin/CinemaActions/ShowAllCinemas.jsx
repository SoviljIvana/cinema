import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table, Container } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';

class ShowAllCinemas extends Component {
    constructor(props) {
        super(props);
        this.state = {
            cinemas: [],
            isLoading: true
        };
        this.editCinema = this.editCinema.bind(this);
        this.removeCinema = this.removeCinema.bind(this);
    }

    componentDidMount() {
        this.getCinemas();
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

    removeCinema(id) {
        const requestOptions = {
            method: 'DELETE',
            headers: {
                'Content-Type': 'application/json',
                'Authorization': 'Bearer ' + localStorage.getItem('jwt')
            }
        };

        fetch(`${serviceConfig.baseURL}/api/cinemas/${id}`, requestOptions)
            .then(response => {
                if (!response.ok) {
                    return Promise.reject(response);
                }
                return response.statusText;
            })
            .then(result => {
                NotificationManager.success('Successfuly removed cinema ');
                const newState = this.state.cinemas.filter(cinema => {
                    return cinema.id !== id;
                })
                this.setState({ cinemas: newState });
            })
            .catch(response => {
                NotificationManager.error("Unable to remove cinema");
                this.setState({ submitted: false });
            });
    }

    fillTableWithDaata() {
        return this.state.cinemas.map(cinema => {
            return <tr key={cinema.id}>
                <td  >{cinema.name}</td>
                <td width="1%" className="text-center cursor-pointer" onClick={() => this.editCinema(cinema.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit} /></td>
                <td width="1%" className="text-center cursor-pointer" onClick={() => this.removeCinema(cinema.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash} /></td>
            </tr>
        })
    }

    editCinema(id) {
        this.props.history.push(`editcinema/${id}`);
    }

    render() {
        const { isLoading } = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
            <thead>
                <tr>
                    <th>Name</th>
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
             <Row className="no-gutters pr-5 pl-5">
              {showTable}
             </Row>
        );
    }
}

export default ShowAllCinemas;