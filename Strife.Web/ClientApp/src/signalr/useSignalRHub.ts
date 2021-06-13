import { HubConnection, HubConnectionState } from "@microsoft/signalr";
import { useContext, useEffect, useState } from "react";
import { SignalRHubContext } from "./SignalRHubContext";

export interface SignalRHubResponse {
    connectionState: HubConnectionState;
    connectionId: Nullable<string>;
}

export const SignalRHubMethods = {
    Guild: {
        Created: "Guilds/Created",
        UserAdded: "Guilds/UserAdded"
    },
    Role: {
        Created: "Roles/Created",
        UserAdded: "Roles/UserAdded"
    },
    Channel: {
        Created: "Channels/Created",
        MetaUpdated: "Channels/MetaUpdated",
    },
    Message: {
        Created: "Messages/Created"
    }
};

export const useSignalRHub = (method: string, callback: (...args: any[]) => void, usecase?: string): SignalRHubResponse => {
    const { connection: { current: connection }, started } = useContext(SignalRHubContext);
    const [connectionState, setConnectionState] = useState(connection.state);
    const [connectionId, setConnectionId] = useState<Nullable<string>>(null);
    
    useEffect(() => {
        connection.on(method, callback);
        return () => {
            connection.off(method);
        };
    }, [callback, connection, method]);

    useEffect(() => {
        if (!started.current) {
            const stateChangeHandler = () => {
                setConnectionState(connection.state);
                setConnectionId(connection.connectionId);
            };
            connection.onclose(stateChangeHandler);
            connection.onreconnecting(stateChangeHandler);
            connection.onreconnected(stateChangeHandler);
            connection.start().then(() => {
                setConnectionState(connection.state);
                setConnectionId(connection.connectionId);
            });
            started.current = true;
        }
        
        if (!connectionId && connection.connectionId) {
            setConnectionId(connection.connectionId);
        }
    }, [connection, started, method, callback]);

    return {
        connectionState,
        connectionId,
    };
};