import React, { Component } from 'react';
import { Row, Col } from 'react-bootstrap';
import { Switch, NavLink } from 'react-router-dom';
import {  Badge } from 'react-bootstrap';
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
import { PrivateRouteAdmin } from '../hoc/privateRouteAdmin';
import {PrivateRouteSuperUser} from '../hoc/privateRouteSuperUser';

class Dashboard extends Component {
    render() {
        return (
            <Row className="justify-content-center no-gutters">
                <Col lg={2} className="dashboard-navigation">
                  <br></br>
                    <Row className="justify-content-center">
                       <Badge pill variant="light"><h2>CINEMA</h2></Badge>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllCinemas'><Badge pill variant="dark"><h5>ALL CINEMAS</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewCinema'><Badge pill variant="dark"><h5>ADD CINEMA</h5></Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center mt-2">
                        <Badge pill variant="light"><h2> MOVIE </h2></Badge>
                        </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllMovies'><Badge pill variant="info"><h5>ALL MOVIES</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewMovie'><Badge pill variant="info"><h5>ADD MOVIE</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/TopTenMovies'><Badge pill variant="info"><h5>TOP 10 MOVIES</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/ShowCurrentMovies'><Badge pill variant="info"><h5>CURRENT MOVIES</h5></Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center"> <Badge pill variant="light"> <h2>PROJECTION</h2></Badge>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllProjections'><Badge pill variant="secondary"><h5> ALL PROJECTIONS</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/FilterProjections'><Badge pill variant="secondary"><h5> FILTER PROJECTIONS</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewProjection'><Badge pill variant="secondary"><h5>ADD PROJECTION</h5></Badge></NavLink>
                    </Row>
                    <br></br>
                    <Row className="justify-content-center">
                     <Badge pill variant="light"><h2> AUDITORIUM</h2></Badge>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/AllAuditoriums'><Badge pill variant="danger"><h5>ALL AUDITORIUMS</h5></Badge></NavLink>
                    </Row>
                    <Row className="justify-content-center mt-2">
                        <NavLink activeClassName="active-link" to='/dashboard/NewAuditorium'><Badge pill variant="danger"> <h5>ADD AUDITORIUM</h5></Badge></NavLink>
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