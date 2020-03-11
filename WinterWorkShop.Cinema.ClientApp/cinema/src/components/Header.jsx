import React, { Component } from 'react';
import { Link } from 'react-router-dom';
import { Navbar, Nav, Form, FormControl, Button, Container, NavDropdown } from 'react-bootstrap';
import { NotificationManager } from 'react-notifications';
import { serviceConfig } from '../appSettings';
import { FaUserAlt } from "react-icons/fa";

class Header extends Component {
  constructor(props) {
    super(props);
    this.state = {
      username: '',
      user:[],
      submitted: false
    };
    this.handleChange = this.handleChange.bind(this);
    this.handleSubmit = this.handleSubmit.bind(this);
  }

  componentDidMount(){
    this.guestToken();
  }

  handleChange(e) {
    const { id, value } = e.target;
    this.setState({ [id]: value });
  }

  handleSubmit(e) {
    e.preventDefault();
    this.setState({ submitted: true });
    //const { username,user } = this.state;
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
              }else if(this.state.user.isAdmin == false) {
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
      method: 'GET'
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=gost&guest=true&admin=false&superUser=false`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        NotificationManager.success('Singed in as guest!');
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
        NotificationManager.error(response.message || response.statusText);
        this.setState({ submitted: false });
      });
  }
  userLogin() {
    const { username } = this.state;

    const requestOptions = {
      method: 'GET'
    };

    fetch(`${serviceConfig.baseURL}/get-token?name=${username}`, requestOptions)
      .then(response => {
        if (!response.ok) {
          return Promise.reject(response);
        }
        return response.json();
      })
      .then(data => {
        NotificationManager.success('Successfully signed in as'+ this.state.user.firstName + '!');
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
          </Form>
        </Navbar.Collapse>
      </Navbar>
    );
  }
}

export default Header;