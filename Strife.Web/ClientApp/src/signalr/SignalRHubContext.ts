import { createContext, MutableRefObject, useRef } from "react";
import { HubConnection } from "@microsoft/signalr";
import * as signalR from "@microsoft/signalr";
import { hostnames } from "../api/http/base";
import authorizationService from "../oidc/AuthorizationService";

export interface ISignalRHubContext {
    connection: MutableRefObject<HubConnection>;
    started: MutableRefObject<boolean>;
}

export const SignalRHubContext = createContext<ISignalRHubContext>({
    connection: {
        current: new signalR.HubConnectionBuilder()
            .withUrl(`${hostnames.api}/hub`, {
                accessTokenFactory: async () => await authorizationService.getAccessToken(),
            })
            .build()
    },
    started: { current: false },
});