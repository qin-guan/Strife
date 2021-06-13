import * as React from "react";
import { createRef, FC, useCallback, useEffect, useRef, useState } from "react";

import { Center, Heading, Spinner, Tag, useToast } from "@chakra-ui/react";
import { VariableSizeList, VariableSizeList as List } from "react-window";

import { useChannelMeta } from "../../../api/swr/channels";

import MessagePage from "./MessagePage";
import { SignalRHubMethods, useSignalRHub } from "../../../signalr/useSignalRHub";

export interface MessagesListProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const MessagesList: FC<MessagesListProps> = (props) => {
    const { selectedGuild, selectedChannel } = props;

    const { data: meta, error, mutate: mutateMeta } = useChannelMeta(selectedGuild, selectedChannel);
    
    useSignalRHub(SignalRHubMethods.Channel.MetaUpdated, (_, channelId) => {
        if (channelId !== selectedChannel) return;
        mutateMeta();
    });

    const [pageCount, setPageCount] = useState(1);
    const [firstRender, setFirstRender] = useState(true);

    const sizeMap = useRef<Record<number, number>>({});
    const messageListRef = createRef<VariableSizeList>();

    useEffect(() => {
        setPageCount(1);
        setFirstRender(true);
    }, [selectedChannel]);

    const setPageHeight = useCallback((page: number, height: number) => {
        sizeMap.current[page] = height;
    }, []);
    const getPageHeight = useCallback((page: number) => {
        if (!meta?.PageCount || page === 0) {
            return 300;
        }

        return sizeMap.current[meta?.PageCount - pageCount + page] ?? 1440;
    }, [meta, pageCount]);

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

    if (!meta) {
        return (
            <Center style={{ flex: 1 }}>
                <Spinner size={"lg"}/>
            </Center>
        );
    }

    if (meta.PageCount === 0) {
        return (
            <Center style={{ flex: 1 }}>
                <Heading size={"md"}>No messages, send one!</Heading>
            </Center>
        );
    }

    const onScroll = ({ scrollOffset }: { scrollOffset: number }) => {
        if (scrollOffset < 300 && pageCount < meta.PageCount) {
            setPageCount(p => p + 1);
            messageListRef.current && messageListRef.current.scrollToItem(2, "start");
        }
    };
    const onItemsRendered = () => {
        if (!firstRender) return;
        setFirstRender(false);
        messageListRef.current && messageListRef.current.scrollToItem(1, "start");
    };

    return (
        <>
            <List
                ref={messageListRef}
                width={"calc(100vw - 384px)"}
                itemSize={getPageHeight}
                itemCount={pageCount + 1}
                height={window.innerHeight - 48}
                onScroll={onScroll}
                onItemsRendered={onItemsRendered}
            >
                {({ index, style }) => {
                    if (index === 0) {
                        return (
                            <Center style={{ height: 300 }}>
                                <Tag size={"lg"} variant="solid" colorScheme="teal">
                                    {pageCount < meta.PageCount ? "Loading..." : "No more messages"}
                                </Tag>
                            </Center>
                        );
                    }
                    return (
                        <MessagePage
                            style={style}
                            selectedGuild={selectedGuild}
                            selectedChannel={selectedChannel}
                            page={meta?.PageCount - pageCount + index}

                            setHeight={setPageHeight}
                        />
                    );
                }}
            </List>
            {pageCount}
        </>
    );
};

export default MessagesList;