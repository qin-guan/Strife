import * as React from "react"
import {useEffect, useState} from "react"

import {RouteComponentProps, useHistory} from 'react-router';

import {LoginActions, OidcPaths, QueryParameterNames} from "../../oidc/AuthorizationConstants";
import authorizationService, {AuthenticationResultStatus} from "../../oidc/AuthorizationService"

interface MatchParams {
  [key: string]: string;
}

interface LoginProps extends Partial<RouteComponentProps<MatchParams>> {
  action: string
}

export const Login: React.FC<LoginProps> = (props) => {
  const [message, setMessage] = useState<string>()
  const history = useHistory()

  const {
    action,
    match: {params}
      = {params: {}}
  } = props

  useEffect(() => {
    const getAction = async () => {
      switch (action) {
        case LoginActions.Login:
          await login(getReturnUrl());
          break;
        case LoginActions.LoginCallback:
          await processLoginCallback();
          break;
        case LoginActions.LoginFailed:
          const error = params[QueryParameterNames.Message];
          setMessage(error)
          break;
        case LoginActions.Profile:
          redirectToProfile();
          break;
        case LoginActions.Register:
          redirectToRegister();
          break;
        default:
          throw new Error(`Invalid action '${action}'`);
      }
    }

    getAction()
  }, [])

  async function login(returnUrl: string) {
    const state = {returnUrl};
    const result: { status: string, message?: string, state?: object } = await authorizationService.signIn(state);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        break;
      case AuthenticationResultStatus.Success:
        await navigateToReturnUrl(returnUrl);
        break;
      case AuthenticationResultStatus.Fail:
        setMessage(result.message);
        break;
      default:
        throw new Error(`Invalid status result ${result.status}.`);
    }
  }

  async function processLoginCallback() {
    const url = window.location.href;
    const result: { status: string, message?: string, state?: { returnUrl?: string } } = await authorizationService.completeSignIn(url);
    switch (result.status) {
      case AuthenticationResultStatus.Redirect:
        // There should not be any redirects as the only time completeSignIn finishes
        // is when we are doing a redirect sign in flow.
        throw new Error('Should not redirect.');
      case AuthenticationResultStatus.Success:
        await navigateToReturnUrl(getReturnUrl(result.state));
        break;
      case AuthenticationResultStatus.Fail:
        setMessage(result.message)
        break;
      default:
        throw new Error(`Invalid authentication result status '${result.status}'.`);
    }
  }

  function getReturnUrl(state?: { returnUrl?: string }) {
    if (QueryParameterNames.ReturnUrl in params) {
      if (!params[QueryParameterNames.ReturnUrl].startsWith(`${window.location.origin}/`)) {
        throw new Error("Invalid return url. The return url needs to have the same origin as the current page.")
      }
    }

    return (state && state.returnUrl) || params[QueryParameterNames.ReturnUrl] || `${window.location.origin}/`;
  }

  function redirectToRegister() {
    redirectToApiAuthorizationPath(`${OidcPaths.IdentityRegisterPath}?${QueryParameterNames.ReturnUrl}=${encodeURI(OidcPaths.Login)}`);
  }

  function redirectToProfile() {
    redirectToApiAuthorizationPath(OidcPaths.IdentityManagePath);
  }

  function redirectToApiAuthorizationPath(apiAuthorizationPath: string) {
    const redirectUrl = `${window.location.origin}/${apiAuthorizationPath}`;
    // It's important that we do a replace here so that when the user hits the back arrow on the
    // browser they get sent back to where it was on the app instead of to an endpoint on this
    // component.
    history.replace(redirectUrl);
  }

  function navigateToReturnUrl(returnUrl: string) {
    // It's important that we do a replace here so that we remove the callback uri with the
    // fragment containing the tokens from the browser history.
    history.replace(returnUrl);
  }

  if (!!message) {
    return <div>{message}</div>
  } else {
    switch (action) {
      case LoginActions.Login:
        return (<div>Processing login</div>);
      case LoginActions.LoginCallback:
        return (<div>Processing login callback</div>);
      case LoginActions.Profile:
      case LoginActions.Register:
        return (<div></div>);
      default:
        throw new Error(`Invalid action '${action}'`);
    }
  }
}
