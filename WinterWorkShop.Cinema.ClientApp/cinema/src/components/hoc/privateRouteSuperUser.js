import React, { useEffect } from "react";
import { Route, Redirect } from "react-router-dom";
import { NotificationManager } from 'react-notifications';
import * as superUserCheck from '../helpers/superUserCheck';

export const PrivateRouteSuperUser = ({ component: Component, ...rest }) => {
    useEffect(() => {
        if(!superUserCheck.isSuperUser()){
            NotificationManager.error('Access not allowed!');
        }
      });
    return (
    <Route {...rest} render={ props => localStorage.getItem('jwt') && superUserCheck.isSuperUser() ? ( <Component {...props} />) : 
            ( <Redirect to={{ pathname: "/", state: { from: props.location } }} />
        )}/>
    )
}