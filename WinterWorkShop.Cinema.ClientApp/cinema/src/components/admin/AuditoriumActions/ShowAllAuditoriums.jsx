import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';

class ShowAllAuditoriums extends Component {
    constructor(props) {
      super(props);
      this.state = {
        auditoriums: [],
        isLoading: true
      };
      this.editAuditorium = this.editAuditorium.bind(this);
      this.removeAuditorium = this.removeAuditorium.bind(this);
    }
    
    componentDidMount() {
      this.getAuditoriums();
    }

    getAuditoriums() {
      const requestOptions = {
        method: 'GET',
        headers: {'Content-Type': 'application/json',
                      'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
      };

      this.setState({isLoading: true});
      fetch(`${serviceConfig.baseURL}/api/Auditoriums/all`, requestOptions)
        .then(response => {
          if (!response.ok) {
            return Promise.reject(response);
        }
        return response.json();
        })
        .then(data => {
          if (data) {
            this.setState({ 
                auditoriums: data,
                 isLoading: false });
            }
        })
        .catch(response => {
            NotificationManager.error(response.message || response.statusText);
            this.setState({ isLoading: false });
        });
    }

    removeAuditorium(id) {
      const requestOptions = {
        method: 'DELETE',
        headers: {'Content-Type': 'application/json',
                  'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
    };

    fetch(`${serviceConfig.baseURL}/api/auditoriums/${id}`, requestOptions)
        .then(response => {
            if (!response.ok) {
                return Promise.reject(response);
            }
            return response.statusText;
        })
        .then(result => {
            NotificationManager.success('Successfuly removed auditorium with ID: '+ id);
            const newState = this.state.auditoriums.filter(auditorium => {
                return auditorium.id !== id;
            })
            this.setState({auditoriums: newState});
        })
        .catch(response => {
            NotificationManager.error("Unable to remove auditorium.");
            this.setState({ submitted: false });
        });
    }

    fillTableWithDaata() {
        return this.state.auditoriums.map(auditorium => {
            return <tr key={auditorium.id}>
                        <td>{auditorium.name}</td>
                        <td >{auditorium.cinemaName}</td>
                        <td width = "1%" className="text-center cursor-pointer"  onClick={() => this.editAuditorium(auditorium.id)}><FontAwesomeIcon className="text-info mr-2 fa-1x" icon={faEdit}/></td>
                        <td  width = "1%" className="text-center cursor-pointer" onClick={() => this.removeAuditorium(auditorium.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash}/></td>
                     </tr>
        })
    }

    editAuditorium(id) {
        this.props.history.push(`editauditorium/${id}`);
    }

    render() {
        const {isLoading} = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table class="tablesaw tablesaw-stack" data-tablesaw-mode="stack">
                            <thead>
                                <th>Name</th>
                                <th >Cinema Name</th>
                                <th className="text-center cursor-pointer">Edit</th>
                                <th className="text-center cursor-pointer">Delete</th>
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
export default ShowAllAuditoriums;