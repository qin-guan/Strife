import React from 'react';
import {
    Switch,
    Route,
    Link
} from "react-router-dom";

import { Flex } from "@chakra-ui/react"

import { Landing } from "./pages"

import { LandingNavBar } from "./components/NavBars"

function App() {
    return (
        <Switch>
            <Flex direction={"column"}>
                <Route path="/">
                    <LandingNavBar />
                    <Landing />
                </Route>
            </Flex>
        </Switch>
    );
}

export default App;
