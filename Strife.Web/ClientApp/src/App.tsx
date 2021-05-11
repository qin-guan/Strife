import React from "react";
import {
    Switch
} from "react-router-dom";
import { Flex } from "@chakra-ui/react";

import Routes from "./routes";

// import { Provider, rootStore } from "./models/root/Root";

import { store } from "./store";
import { Provider } from "react-redux";

function App(): React.ReactElement {
    return (
        <Provider store={store}>
            <Switch>
                <Flex w={"100vw"} h={"100vh"} direction={"column"}>
                    <Routes/>
                </Flex>
            </Switch>
        </Provider>
    );
}

export default App;
