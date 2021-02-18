import * as React from "react"
import {Route, Switch} from "react-router-dom";
import {OidcPaths} from "../oidc/AuthorizationConstants";

import {LandingRoutes} from "./LandingRoutes";
import {WebAppRoutes} from "./WebAppRoutes";
import {AuthorizationRoutes} from "./AuthorizationRoutes";

import {NotFound} from "../pages/exceptions/NotFound";

function Routes() {
  return (
    <Switch>
      <Route exact path={"/"} component={LandingRoutes}/>
      <Route path={OidcPaths.ApiAuthorizationPrefix} component={AuthorizationRoutes}/>
      <Route path={"/app"} component={WebAppRoutes}/>

      <Route component={NotFound}/>
    </Switch>
  )
}

export default Routes
