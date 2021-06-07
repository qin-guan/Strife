import * as React from "react";
import { CSSProperties, memo } from "react";

import { Box, Flex, Spinner, useToast } from "@chakra-ui/react";
import { areEqual } from "react-window";

import { useMessages } from "../../../api/swr/messages";

import Message from "./Message";
import { SignalRHubMethods, useSignalRHub } from "../../../signalr/useSignalRHub";
import { HubConnectionState } from "@microsoft/signalr";

export interface MessagePageProps {
    selectedGuild: string;
    selectedChannel: string;
    style: CSSProperties;
    page: number;
}

const MessagePage = (props: MessagePageProps) => {
    const { selectedGuild, selectedChannel, page, style } = props;
    const { data, error, mutate } = useMessages(selectedGuild, selectedChannel, page);
    const { connectionState, connectionId } = useSignalRHub(SignalRHubMethods.Message.Created, () => mutate());
    
    const toast = useToast();
    
    if (error) {
        toast({
            title: "Unknown exception",
            description: "An unknown exception occurred while fetching your messages",
            status: "error",
            isClosable: true,
        });
        return null;
    }

    if (!connectionId || connectionState !== HubConnectionState.Connected || !data) {
        return <Spinner/>;
    }

    return (
        <Box style={style}>
            {data.map((message, idx) => (
                <Message key={idx} content={message.Content} user={message.SenderId}/>
            ))}
        </Box>
    );
};

export default memo(MessagePage, areEqual);