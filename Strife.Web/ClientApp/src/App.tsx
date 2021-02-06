import React from 'react';
import {
    Switch,
    Route,
    Link
} from "react-router-dom";

import { Landing } from "./pages"

function App() {
    return (
        <Switch>
            <Route path="/">
                <Landing />
            </Route>
        </Switch>
    );
}

export default App;
