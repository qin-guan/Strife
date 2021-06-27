import * as React from "react";
import { createRef, CSSProperties, memo, useEffect } from "react";

import { Box, Spinner, useToast } from "@chakra-ui/react";
import { HubConnectionState } from "@microsoft/signalr";

import { useMessages } from "../../../api/swr/messages";

import Message from "./Message";
import { SignalRHubMethods, useSignalRHub } from "../../../signalr/useSignalRHub";
import { useWindowSize } from "../../../hooks/useWindowSize";
import { useChannelMeta } from "../../../api/swr/channels";
import MessagePlaceholder from "./MessagePlaceholder";
import { areEqual } from "react-window";

export interface MessagePageProps {
    selectedGuild: string;
    selectedChannel: string;
    style: CSSProperties;
    page: number;

    setHeight: (page: number, height: number) => void;
}

const MessagePage = (props: MessagePageProps) => {
    const { selectedGuild, selectedChannel, page, style, setHeight } = props;
    const { data: meta, error: metaError } = useChannelMeta(selectedGuild, selectedChannel);
    const {
        data = meta ? Array.from({ length: meta.PageCount }) : [],
        error,
        mutate
    } = useMessages(selectedGuild, selectedChannel, page);

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
    }, [containerRef, setHeight, page, width, selectedChannel]);

    if (error || metaError) {
        toast({
            title: "Unknown exception",
            description: "An unknown exception occurred while fetching your messages in the page",
            status: "error",
            isClosable: true,
        });
        return null;
    }

    if (!connectionId || connectionState !== HubConnectionState.Connected) {
        return <Spinner/>;
    }

    return (
        <Box style={style}>
            <div ref={containerRef}>
                {data.map((message) => {
                    if (!message) {
                        return (
                            <MessagePlaceholder/>
                        );
                    }
                    return (
                        <Message
                            key={message.Id}
                            content={message.Content}
                            dateSent={message.DateSent}
                            user={message.SenderId}
                        />
                    );
                })}
            </div>
        </Box>
    );
};

export default memo(MessagePage, areEqual);