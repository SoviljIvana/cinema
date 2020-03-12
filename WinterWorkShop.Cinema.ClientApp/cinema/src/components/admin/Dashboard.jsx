import React, { Component } from 'react';
import { Row, Col } from 'react-bootstrap';
import { Switch, NavLink, } from 'react-router-dom';
import { NavbarBrand, Badge } from 'react-bootstrap';

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


class Dashboard extends Component {
    render() {
        return (
            <Row className="justify-content-center no-gutters">
                <Col lg={2} className="dashboard-navigation">
                    <Row className="justify-content-center mt-2">
                        <h2><Badge pill variant="dark">
                            MOVIE</Badge></h2> </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllMovies'><Badge pill variant="info"> ALL MOVIES</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewMovie'><Badge pill variant="info">ADD MOVIE</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/TopTenMovies'><Badge pill variant="info">TOP 10 MOVIES </Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/ShowCurrentMovies'><Badge pill variant="info">CURRENT MOVIES</Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center">
                        <h2><Badge pill variant="dark"> AUDITORIUM</Badge></h2>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllAuditoriums'><Badge pill variant="info">ALL AUDITORIUMS</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewAuditorium'><Badge pill variant="info"> ADD AUDITORIUM</Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center">
                        <h2><Badge pill variant="dark">CINEMA</Badge></h2>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllCinemas'><Badge pill variant="info"> ALL CINEMAS</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewCinema'><Badge pill variant="info">ADD CINEMA</Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center"> <h2><Badge pill variant="dark"> PROJECTION </Badge></h2>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllProjections'><Badge pill variant="info"> ALL PROJECTIONS</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/FilterProjections'><Badge pill variant="info"> FILTER PROJECTIONS</Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewProjection'><Badge pill variant="info">ADD PROJECTIONS</Badge></NavLink>
                    </Row>
                </Col>
                <Col className="pt-2 app-content-main">
                    <Switch>
                        <PrivateRouteAdmin path="/dashboard/NewMovie" component={NewMovie} />
                        <PrivateRouteAdmin path="/dashboard/AllMovies" component={ShowAllMovies} />
                        <PrivateRouteAdmin path="/dashboard/ShowCurrentMovies" component={ShowCurrentMovies} />
                        <PrivateRouteAdmin path="/dashboard/TopTenMovies" component={TopTenMovies} />
                        <PrivateRouteAdmin path="/dashboard/EditMovie/:id" component={EditMovie} />
                        <PrivateRouteAdmin path="/dashboard/NewCinema" component={NewCinema} />
                        <PrivateRouteAdmin path="/dashboard/EditCinema/:id" component={EditCinema} />
                        <PrivateRouteAdmin path="/dashboard/AllCinemas" component={ShowAllCinemas} />
                        <PrivateRouteAdmin path="/dashboard/NewAuditorium" component={NewAuditorium} />
                        <PrivateRouteAdmin path="/dashboard/EditAuditorium/:id" component={EditAuditorium} />
                        <PrivateRouteAdmin path="/dashboard/AllAuditoriums" component={ShowAllAuditoriums} />
                        <PrivateRouteAdmin path="/dashboard/AllProjections" component={ShowAllProjections} />
                        <PrivateRouteAdmin path="/dashboard/FilterProjections" component={FilterProjections} />
                        <PrivateRouteAdmin path="/dashboard/NewProjection" component={NewProjection} />
                    </Switch>
                </Col>
            </Row>
        );
    }
}

export default Dashboard;