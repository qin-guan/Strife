import React from "react";
import {
    Switch
} from "react-router-dom";
import { Flex } from "@chakra-ui/react"

import Routes from "./routes";

import { Provider, rootStore } from "./models/root/Root"

function App() {
    return (
        <Provider value={rootStore}>
            <Switch>
                <Flex w={"100vw"} h={"100vh"} direction={"column"}>
                    <Routes/>
                </Flex>
            </Switch>
        </Provider>
    );
}

export default App;
