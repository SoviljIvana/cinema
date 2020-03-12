import React, { Component } from 'react';
import { Row, Col } from 'react-bootstrap';
import { Switch, NavLink } from 'react-router-dom';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faPlus, faList, faFilm, faVideo, faTicketAlt, faBinoculars } from '@fortawesome/free-solid-svg-icons';

// Admin actions
import NewMovie from './MovieActions/NewMovie';
import EditMovie from './MovieActions/EditMovie';
import ShowAllMovies from './MovieActions/ShowAllMovies';
import ShowCurrentMovies from './MovieActions/ShowCurrentMovies';
import TopTenMovies from './MovieActions/TopTenMovies';
import NewCinema from './CinemaActions/NewCinema';
import EditCinema from './CinemaActions/EditCinema';
import ShowAllCinemas from './CinemaActions/ShowAllCinemas';
import NewAuditorium from './AuditoriumActions/NewAuditorium';
import EditAuditorium from './AuditoriumActions/EditAuditorium';
import ShowAllAuditoriums from './AuditoriumActions/ShowAllAuditoriums';
import ShowAllProjections from './ProjectionActions/ShowAllProjections';
import NewProjection from './ProjectionActions/NewProjection';
import FilterProjections from './ProjectionActions/FilterProjections';


// higher order component
import { PrivateRouteAdmin } from '../hoc/privateRouteAdmin';
import {PrivateRouteSuperUser} from '../hoc/privateRouteSuperUser';


class Dashboard extends Component {
    render() {
        return (
            <Row className="justify-content-center no-gutters">
                <Col lg={2} className="dashboard-navigation">
                    <Row className="justify-content-center mt-2">
                        <span className="fa-2x text-white"><FontAwesomeIcon className="text-white mr-2 fa-1x" icon={faFilm}/>Movie</span>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllMovies'><FontAwesomeIcon className='text-primary mr-1' icon={faList}/>All Movies</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewMovie'><FontAwesomeIcon className='text-primary mr-1' icon={faPlus}/>Add Movie</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/TopTenMovies'><FontAwesomeIcon className='text-primary mr-1' />Top 10 Movies</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/ShowCurrentMovies'><FontAwesomeIcon className='text-primary mr-1' />Current Movies</NavLink>
                    </Row>
                    <Row className="justify-content-center">
                        <span className="fa-2x text-white"><FontAwesomeIcon className="text-white mr-2 fa-1x" icon={faBinoculars}/>Auditorium</span>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllAuditoriums'><FontAwesomeIcon className='text-primary mr-1' icon={faList}/>All Auditoriums</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewAuditorium'><FontAwesomeIcon className='text-primary mr-1' icon={faPlus}/>Add Auditorium</NavLink>
                    </Row>
                    <Row className="justify-content-center">
                        <span className="fa-2x text-white"><FontAwesomeIcon className="text-white mr-2 fa-1x" icon={faTicketAlt}/>Cinema</span>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllCinemas'><FontAwesomeIcon className='text-primary mr-1' icon={faList}/>All Cinemas</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewCinema'><FontAwesomeIcon className='text-primary mr-1' icon={faPlus}/>Add Cinema</NavLink>
                    </Row>
                    <Row className="justify-content-center">
                        <span className="fa-2x text-white"><FontAwesomeIcon className="text-white mr-2 fa-1x" icon={faVideo}/>Projection</span>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllProjections'><FontAwesomeIcon className='text-primary mr-1' icon={faList}/>All Projections</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/FilterProjections'><FontAwesomeIcon className='text-primary mr-1' icon={faList}/>Filter Projections</NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewProjection'><FontAwesomeIcon className='text-primary mr-1' icon={faPlus}/>Add Projection</NavLink>
                    </Row>
                </Col>
                <Col className="pt-2 app-content-main">
                    <Switch>
                        <PrivateRouteSuperUser path="/dashboard/NewMovie" component={NewMovie} />
                        <PrivateRouteSuperUser path="/dashboard/AllMovies" component={ShowAllMovies} />
                        <PrivateRouteSuperUser path="/dashboard/ShowCurrentMovies" component={ShowCurrentMovies} />
                        <PrivateRouteSuperUser path="/dashboard/TopTenMovies" component={TopTenMovies} />
                        <PrivateRouteSuperUser path="/dashboard/EditMovie/:id" component={EditMovie} />
                        <PrivateRouteAdmin path="/dashboard/NewCinema" component={NewCinema} />
                        <PrivateRouteAdmin path="/dashboard/EditCinema/:id" component={EditCinema} />
                        <PrivateRouteSuperUser path="/dashboard/AllCinemas" component={ShowAllCinemas} />
                        <PrivateRouteAdmin path="/dashboard/NewAuditorium" component={NewAuditorium} />
                        <PrivateRouteAdmin path="/dashboard/EditAuditorium/:id" component={EditAuditorium} />
                        <PrivateRouteSuperUser path="/dashboard/AllAuditoriums" component={ShowAllAuditoriums} />
                        <PrivateRouteSuperUser path="/dashboard/AllProjections" component={ShowAllProjections} />
                        <PrivateRouteSuperUser path="/dashboard/FilterProjections" component={FilterProjections}/>
                        <PrivateRouteSuperUser path="/dashboard/NewProjection" component={NewProjection} />
                    </Switch>
                </Col>
            </Row>
        );
      }
}

export default Dashboard;