import * as React from "react"
import {useEffect, useState} from "react"

import {RouteComponentProps, useHistory} from 'react-router';

import {LogoutActions, OidcPaths, QueryParameterNames} from "../../oidc/AuthorizationConstants";
import authorizationService, {AuthenticationResultStatus} from "../../oidc/AuthorizationService"

interface MatchParams {
  [key: string]: string;
}

interface LogoutProps extends Partial<RouteComponentProps<MatchParams>> {
  action: string
}

export const Logout: React.FC<LogoutProps> = (props) => {
  const [isReady, setIsReady] = useState(false)
  const [message, setMessage] = useState<string>()
  const [authenticated, setAuthenticated] = useState(false)

  const history = useHistory()

  const {
    action,
    match: {params}
      = {params: {}}
  } = props

  useEffect(() => {
    switch (action) {
      case LogoutActions.Logout:
        if (!!window.history.state.state.local) {
          logout(getReturnUrl());
        } else {
          // This prevents regular links to <app>/authentication/logout from triggering a logout
          setIsReady(true)
          setMessage("The logout was not initiated from within the page.")
        }
        break;
      case LogoutActions.LogoutCallback:
        processLogoutCallback();
        break;
      case LogoutActions.LoggedOut:
        setIsReady(true)
        setMessage("You successfully logged out!")
        break;
      default:
        throw new Error(`Invalid action '${action}'`);
    }

    populateAuthenticationState();
  }, [])


  async function logout(returnUrl: string) {
    const state = { returnUrl };
    const isAuthenticated = await authorizationService.isAuthenticated();
    if (isAuthenticated) {
      const result: {status: string, message?: string} = await authorizationService.signOut(state);
      switch (result.status) {
        case AuthenticationResultStatus.Redirect:
          break;
        case AuthenticationResultStatus.Success:
          await navigateToReturnUrl(returnUrl);
          break;
        case AuthenticationResultStatus.Fail:
          setMessage(result.message)
          break;
        default:
          throw new Error("Invalid authentication result status.");
      }
    } else {
      setMessage("You successfully logged out!")
    }
  }

  async function processLogoutCallback() {
    const result: {status: string, state?: object, message?: string} = await authorizationService.completeSignOut(window.location.href);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeAuthentication finishes
        // is when we are doing a redirect sign in flow.
        throw new Error('Should not redirect.');
      case AuthenticationResultStatus.Success:
        await navigateToReturnUrl(getReturnUrl(result.state));
        break;
      case AuthenticationResultStatus.Fail:
        setMessage(result.message)
        break;
      default:
        throw new Error("Invalid authentication result status.");
    }
  }

  async function populateAuthenticationState() {
    const authenticated = await authorizationService.isAuthenticated();
    setIsReady(true)
    setAuthenticated(authenticated)
  }

  function getReturnUrl(state?: { returnUrl?: string }) {
    if (QueryParameterNames.ReturnUrl in params) {
      if (!params[QueryParameterNames.ReturnUrl].startsWith(`${window.location.origin}/`)) {
        throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
      }
    }
    return (state && state.returnUrl) || params[QueryParameterNames.ReturnUrl] || `${window.location.origin}${OidcPaths.LoggedOut}`;
  }

  async function navigateToReturnUrl(returnUrl: string) {
    return history.replace(returnUrl);
  }


  if (!isReady) {
    return <div></div>
  }
  if (!!message) {
    return (<div>{message}</div>);
  } else {
    switch (action) {
      case LogoutActions.Logout:
        return (<div>Processing logout</div>);
      case LogoutActions.LogoutCallback:
        return (<div>Processing logout callback</div>);
      case LogoutActions.LoggedOut:
        return (<div>{message}</div>);
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }
}
