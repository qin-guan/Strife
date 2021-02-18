import * as React from "react";
import {Route, Switch} from "react-router-dom";

import {Home} from "../pages/app/Home"

export function WebAppRoutes() {
    return (
        <Switch>
            <Route path={"/"} component={Home}/>
        </Switch>
    )
}

