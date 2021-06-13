import * as React from "react";
import { createRef, CSSProperties, memo, useEffect } from "react";

import { areEqual } from "react-window";
import { Box, Flex, Spinner, useToast } from "@chakra-ui/react";
import { HubConnectionState } from "@microsoft/signalr";

import { useMessages } from "../../../api/swr/messages";

import Message from "./Message";
import { SignalRHubMethods, useSignalRHub } from "../../../signalr/useSignalRHub";
import { useWindowSize } from "../../../hooks/useWindowSize";

export interface MessagePageProps {
    selectedGuild: string;
    selectedChannel: string;
    style: CSSProperties;
    page: number;
    
    setHeight: (page: number, height: number) => void;
}

const MessagePage = (props: MessagePageProps) => {
    const { selectedGuild, selectedChannel, page, style, setHeight } = props;
    const { data, error, mutate } = useMessages(selectedGuild, selectedChannel, page);
    
    const { connectionState, connectionId } = useSignalRHub(SignalRHubMethods.Message.Created, (_, channelId) => {
        if (channelId !== selectedChannel) return;
        mutate();
    });
    
    const toast = useToast();
    const containerRef = createRef<HTMLDivElement>();
    
    const [width] = useWindowSize();
    
    useEffect(() => {
        if (!containerRef.current) return;
        
        setHeight(page, containerRef.current.getBoundingClientRect().height);
    }, [containerRef, setHeight, page, width]);
    
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
        <Box style={style} ref={containerRef}>
            {data.map((message, idx) => (
                <Message key={idx} content={message.Content} dateSent={message.DateSent} user={message.SenderId}/>
            ))}
        </Box>
    );
};

export default memo(MessagePage, areEqual);