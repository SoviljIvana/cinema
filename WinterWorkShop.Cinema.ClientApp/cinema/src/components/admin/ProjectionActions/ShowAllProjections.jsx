import React, { Component } from 'react';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../../../appSettings';
import { Row, Table } from 'react-bootstrap';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faEdit, faTrash } from '@fortawesome/free-solid-svg-icons';
import Spinner from '../../Spinner';

class ShowAllProjections extends Component {
    constructor(props) {
      super(props);
      this.state = {
        projections: [],
        isLoading: true
      };
      this.editProjection = this.editProjection.bind(this);
      this.removeProjection = this.removeProjection.bind(this);
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
      fetch(`${serviceConfig.baseURL}/api/Projections/all`, requestOptions)
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
                  return projection.id !== id;
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
              
                        <td width="30%">{projection.movieTitle}</td>
                        <td width = '30%'>{projection.auditoriumName}</td>
                        <td width="30%">{projection.projectionTimeString}</td>
                        <td width="10%" className="text-center cursor-pointer" onClick={() => this.removeProjection(projection.id)}><FontAwesomeIcon className="text-danger mr-2 fa-1x" icon={faTrash}/></td>
                    </tr>
        })
    }

    editProjection(id) {
        // to be implemented
        this.props.history.push(`editProjection/${id}`);
    }

    render() {
        const {isLoading} = this.state;
        const rowsData = this.fillTableWithDaata();
        const table = (<Table striped bordered hover size="sm" variant="dark">
                            <thead>
                            <tr>
                                <th>Movie Title</th>
                                <th>Projection Time</th>
                                <th>Projection Time</th>
                                <th>Delete</th>
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
                    <h1 className="form-header ml-2">All Projections</h1>
                </Row>
                <Row className="no-gutters pr-5 pl-5">
                    {showTable}
                </Row>
            </React.Fragment>
        );
      }
}

export default ShowAllProjections;