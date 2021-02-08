import * as React from "react"
import {useEffect, useState} from "react";
import {Box, Flex, Spacer, Button, Heading} from "@chakra-ui/react"
import {NavLink} from "react-router-dom";

import authorizationService from "../../oidc/AuthorizationService"
import {OidcPaths} from "../../oidc/AuthorizationConstants";

export const LandingNavBar = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [userName, setUserName] = useState<string>()

  const subscription = authorizationService.subscribe(() => populateState());

  useEffect(() => {
    populateState();

    return () => {
      authorizationService.unsubscribe(subscription)
    }
  }, [])

  async function populateState() {
    const [isAuthenticated, user] = await Promise.all([authorizationService.isAuthenticated(), authorizationService.getUser()])
    setIsAuthenticated(isAuthenticated)
    user && setUserName(user.name)
  }

  // const profilePath = `${OidcPaths.Profile}`;
  // const logoutPath = {pathname: `${OidcPaths.LogOut}`, state: {local: true}};
  // const registerPath = `${OidcPaths.Register}`;
  // const loginPath = `${OidcPaths.Login}`;

  return (
    <Flex p={"3"} align="center">
      <Box>
        <Heading size={"md"}>Strife</Heading>
      </Box>
      <Spacer/>
      <Box>
        {isAuthenticated ? (
          <Button>Web app</Button>
        ) : (
          <>
            <NavLink to={OidcPaths.Login}>
              <Button mr={4}>Log in</Button>
            </NavLink>
            <NavLink to={OidcPaths.Register}>
              <Button colorScheme={"teal"}>
                Sign Up
              </Button>
            </NavLink>
          </>
        )}
      </Box>
    </Flex>
  )
}
