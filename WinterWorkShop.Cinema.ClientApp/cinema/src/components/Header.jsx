import React, { Component } from 'react';
import { Link, Route } from 'react-router-dom';
import { Navbar, Nav, Form, FormControl, Button, Container, NavDropdown } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { FaUserAlt } from "react-icons/fa";
import {withRouter} from 'react-router-dom';
import { Redirect } from 'react-router-dom';
import { useHistory } from 'react-router-dom';

class Header extends React.Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      user:[],
      submitted: false,
      
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
    this.handleLogOut = this.handleLogOut.bind(this);
  }


  componentDidMount(){
    const token = localStorage.getItem('jwt');
    var jwtDecoder = require('jwt-decode');
    const decodedToken = jwtDecoder(token);
    var role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if((role != 'user') && (role != 'admin') && (role != 'superUser')){
    this.guestToken();}

  }

  handleLogOut(e){
    e.preventDefault();
    this.props.history.push('/projectionlist')
    NotificationManager.warning("Logged out!");
    this.guestToken();

  }

  handleChange(e) {
    const { id, value } = e.target;
    this.setState({ [id]: value });
  }

  handleSubmit(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    this.getUser(this.state.username);
  }
  getUser(username){  
      const requestOptions = {
          method: 'GET',
          headers: {'Content-Type': 'application/json',
                      'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
      };
      fetch(`${serviceConfig.baseURL}/api/users/byusername/${username}`, requestOptions)
          .then(response => {
            if (!response.ok) {
              return Promise.reject(response);
          }
          return response.json();
          })
          .then(data => {
            if (data) {
              this.setState({user:data, isLoading: false})
              if (this.state.user.isAdmin == true) {
                this.adminLogin();
              }else if(this.state.user.isSuperUser == true) {
                  this.superUserLogin();
              }else if((this.state.user.isAdmin == false) && (this.state.user.isSuperUser == false)) {
                this.userLogin();
              }
              }
          })
          .catch(response => {
              NotificationManager.error("Unable to login. ");
              this.setState({ submitted: false });
          });
  }
  guestToken() {
    const requestOptions = {
      method: 'GET',
      headers: {'Content-Type': 'application/json',
                  'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=gost&guest=true&admin=false&superUser=false`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        if (data.token) {
          localStorage.setItem("jwt", data.token);
        }
      })
      .catch(response => {
        NotificationManager.error("Unable to sign in. ");
        this.setState({ submitted: false });
      });
  }
  adminLogin() {
    const { username } = this.state;

    const requestOptions = {
      method: 'GET',
      headers: {'Content-Type': 'application/json',
                  'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=${username}&admin=true`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        NotificationManager.success('Signed in as admin!');
        if (data.token) {
          localStorage.setItem("jwt", data.token);
        }
      })
      .catch(response => {
        NotificationManager.error("Unable to sign in. ");
        this.setState({ submitted: false });
      });
  }
  superUserLogin() {
    const { username } = this.state;

    const requestOptions = {
      method: 'GET',
      headers: {'Content-Type': 'application/json',
                  'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=${username}&superUser=true`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        NotificationManager.success('Signed in as '+ this.state.user.firstName + '!');
        if (data.token) {
          localStorage.setItem("jwt", data.token);
        }
      })
      .catch(response => {
        NotificationManager.error("Unable to sign in. ");
        this.setState({ submitted: false });
      });
  }
  userLogin() {
    const { username } = this.state;

    const requestOptions = {
      method: 'GET',
      headers: {'Content-Type': 'application/json',
                  'Authorization': 'Bearer ' + localStorage.getItem('jwt')}
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=${username}&user=true`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        NotificationManager.success('Successfully signed in as '+ this.state.user.firstName + '!');
        if (data.token) {
          localStorage.setItem("jwt", data.token);
        }
      })
      .catch(response => {
        NotificationManager.error("Unable to sign in. ");
        this.setState({ submitted: false });
      });
  }
  
  render() {
    const { username } = this.state;
    return (
      <Navbar fixed="top" expand="lg" variant="light" bg="light">
        <Nav justify variant="tabs" defaultActiveKey="/projectionlist" className="mr-auto">
          <Container>
            <Navbar.Brand><Nav.Link href='/projectionlist'>What's on</Nav.Link></Navbar.Brand>
            <Navbar.Brand><Nav.Link href='/comingsoon'>Coming soon</Nav.Link></Navbar.Brand>
            <Navbar.Brand><Nav.Link href='/dashboard'>Dashboard</Nav.Link></Navbar.Brand>
            <Navbar.Brand><Nav.Link href='/userProfile'>User profile</Nav.Link></Navbar.Brand>
          </Container>
        </Nav>
        <Navbar.Toggle className="text-white" />
        <Navbar.Collapse id="basic-navbar-nav" className="text-white">
          <Nav className="mr-auto"></Nav>
          <Form inline onSubmit={this.handleSubmit}>
            <FormControl
              type="text"
              placeholder="Username"
              id="username"
              value={username}
              onChange={this.handleChange}
              className="mr-sm-2" />
            <Button type="submit" variant="outline-danger" className="mr-1">Log In</Button>
            <Button type="submit" onClick={this.handleLogOut} variant="outline-danger" className="mr-1">Log Out</Button>
          </Form>
        </Navbar.Collapse>
      </Navbar>
    );
  }
}

export default withRouter(Header);