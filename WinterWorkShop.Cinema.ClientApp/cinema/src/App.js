import React from 'react';
import './App.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import 'react-notifications/lib/notifications.css';
import 'react-bootstrap-typeahead/css/Typeahead.css';
import { Route, Switch, Redirect } from 'react-router-dom';
import { NotificationContainer } from 'react-notifications';

// components
import Header from './components/Header';
import ComingSoon from './components/ComingSoon';
import UserProfile from './components/UserProfile';
import ProjectionDetails from './components/user/ProjectionDetails';
import AllProjectionsForCinema from './components/user/AllProjectionsForCinema';
import AllForProjection from './components/user/AllForProjection';
import ProjectionsFilterForCinema from './components/user/ProjectionsFilterForCinema';
import Tickets from './components/user/Tickets';

import Dashboard from './components/admin/Dashboard';
// higher order component
import { PrivateRouteAdmin } from './components/hoc/privateRouteAdmin';
import {PrivateRouteSuperUser} from './components/hoc/privateRouteSuperUser';

function App() {
  return (
    <React.Fragment>
      <Header/>
      
      <div className="set-overflow-y">
      <Switch>
        <Redirect exact from="/" to="projectionlist" />
        <Route path="/projectiondetails/:id" component={ProjectionDetails} />
        <Route path="/projectionlist" component={AllProjectionsForCinema} />
        <Route path="/projectiondetails/allForProjection/:id" component={AllForProjection} />
        <Route path="/userProfile" component={UserProfile} />
        <Route path="/comingsoon" component={ComingSoon} />
        <Route path="/tickets" component={Tickets} />
        <Route path ="/ProjectionsFilterForCinema" component={ProjectionsFilterForCinema}/>
        <PrivateRouteSuperUser path="/dashboard" component={Dashboard} />

      </Switch>
      <NotificationContainer />
      </div>
    </React.Fragment>
  );
}

export default App;
