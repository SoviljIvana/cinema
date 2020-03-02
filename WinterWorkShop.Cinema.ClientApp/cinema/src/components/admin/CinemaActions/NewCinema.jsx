import React, { Component } from 'react';
import { withRouter } from 'react-router-dom';
import { FormGroup, FormControl, Button, Container, Row, Col, FormText, } from 'react-bootstrap';
import { serviceConfig } from '../../../appSettings';
import { NotificationManager } from 'react-notifications';

class NewCinema extends React.Component {

  constructor(props) {
    super(props);
    this.state = {
      cinemaName: '',
      listOfAuditoriums: [{name:"",seatRows: 0, numberOfSeats: 0 }],
      submitted: false,
      canSubmit: true
    };
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleChange = this.handleChange.bind(this);
    this.change = this.change.bind(this);

  }

  handleChange(e) {
    const { id, value } = e.target;
    this.setState({ [id]: value });
    this.validate(id, value);
  }

  change = (e) => {
    if(["name","seatRows", "numberOfSeats"].includes(e.target.className)){
      let listOfAuditoriums = [...this.state.listOfAuditoriums];
      listOfAuditoriums[e.target.dataset.id][e.target.className] = e.target.value;
      this.setState({listOfAuditoriums}, () => console.log(this.state.listOfAuditoriums));
    }else{
      this.setState({[e.target.name]: e.target.value, [e.target.seatRows]:+e.target.value, [e.target.numberOfSeats]: +e.target.value });
    }
  }

  handleSubmit(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    const { cinemaName, listOfAuditoriums } = this.state;
    if (cinemaName && listOfAuditoriums) {
      this.addCinema();
    } else {
      NotificationManager.error('Please fill in data');
      this.setState({ submitted: false });
    }
  }

  validate(id, value) {
    if (id === 'cinemaName') {
      if (value === '') {
        this.setState({
          titleError: 'Fill in cinema name',
          canSubmit: false
        });
      } else {
        this.setState({
          titleError: '',
          canSubmit: true
        });
      }
    }
  }

  addCinema() {
    const {cinemaName, listOfAuditoriums} = this.state;
    for (let i = 0; i < listOfAuditoriums.length; i++) {
      listOfAuditoriums[i].numberOfSeats = +listOfAuditoriums[i].numberOfSeats;
      listOfAuditoriums[i].seatRows = +listOfAuditoriums[i].seatRows;
    }
    const data = {
      CinemaName: cinemaName,
      listOfAuditoriums: listOfAuditoriums
       };
       
    const requestOptions = {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + localStorage.getItem('jwt')
      },
      body: JSON.stringify(data)
    };

    fetch(`${serviceConfig.baseURL}/api/cinemas/create_empty_cinema`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.statusText;
      })
      .then(result => {
        NotificationManager.success('Successfuly added cinema!');
        this.props.history.push(`AllCinemas`);
      })
      .catch(response => {
        NotificationManager.error(response.message || response.statusText);
        this.setState({ submitted: false });
      });
  }

  addAuditorium = (e) => {
    this.setState((prevState) =>({
      listOfAuditoriums: [...prevState.listOfAuditoriums, {name: "", seatRows: 0, numberOfSeats: 0}]
    })); 
  }

  render() {
    const { cinemaName, submitted, canSubmit } = this.state;
    let {listOfAuditoriums} = this.state
    return (
      <Container>
        <Row>
          <Col>
            <h1 className="form-header">Add New Cinema</h1>
            <form onSubmit={this.handleSubmit}>
              <FormGroup>
                <FormControl
                  id="cinemaName"
                  type="text"
                  placeholder="Cinema Name"
                  value={cinemaName}
                  onChange={this.handleChange}
                />
                <FormGroup>
                  <Button onClick={this.addAuditorium}>Add new auditorium</Button>
                  {
                    listOfAuditoriums.map((val, idx) => {
                      let auditoriumId = 'auditorium-${idx}', seatRowsId = 'seatRows-${idx}', numberofSeatsID = 'seatRows-${idx}'
                      return (
                        <div key={idx}>
                          <label htmlFor={auditoriumId}>{idx+1}. New auditorium name: </label>
                          <input 
                            type = 'text'
                            name = {auditoriumId}
                            data-id = {idx}
                            id = {auditoriumId}
                            value = {listOfAuditoriums[idx].name}
                            onChange = {this.change}
                            className = "name"
                            />

                          <label htmlFor={seatRowsId}> Seat rows: </label>
                          <input 
                            type = 'number'
                            name = {seatRowsId}
                            data-id = {idx}
                            id = {seatRowsId}
                            value = {listOfAuditoriums[idx].seatRows}
                            onChange = {this.change}
                            className = "seatRows"
                            />   

                            <label htmlFor={numberofSeatsID}> Number of seats: </label>
                            <input 
                              type = 'number'
                              name = {numberofSeatsID}
                              data-id = {idx}
                              id = {numberofSeatsID}
                              value = {listOfAuditoriums[idx].numberOfSeats}
                              onChange = {this.change}
                              className = "numberOfSeats"
                              />
                        </div>
                      ) 
                    })
                  }
                </FormGroup>
              </FormGroup>
              <Button type="submit" disabled={submitted || !canSubmit} block>Add Cinema</Button>
            </form>
          </Col>
        </Row>
      </Container>
    );
  }
}

export default withRouter(NewCinema);