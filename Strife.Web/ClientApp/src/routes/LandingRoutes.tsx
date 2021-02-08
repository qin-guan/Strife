import * as React from "react";
import {Route, Switch} from "react-router-dom";

import {Landing} from "../pages/Landing";

export function LandingRoutes() {
  return (
    <Switch>
      <Route path={"/"} component={Landing}/>
    </Switch>
  )
}

