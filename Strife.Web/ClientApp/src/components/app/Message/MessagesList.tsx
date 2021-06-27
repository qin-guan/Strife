import * as React from "react";
import { createRef, FC, useCallback, useEffect, useLayoutEffect, useRef, useState } from "react";

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

    useSignalRHub(SignalRHubMethods.Message.Created, (_, channelId) => {
        if (channelId !== selectedChannel) return;
        setTimeout(() => {
            messageListRef.current && messageListRef.current.scrollTo(pageCount * 10000);
        }, 100);
    });

    const [pageCount, setPageCount] = useState(1);

    const sizeMap = useRef<Map<number, number>>(new Map());
    const messageListRef = createRef<VariableSizeList>();

    useEffect(() => {
        setPageCount(1);
        sizeMap.current = new Map();
    }, [selectedChannel]);

    useLayoutEffect(() => {
        messageListRef.current && messageListRef.current.scrollToItem(1, "smart");
    }, [messageListRef, pageCount]);

    const setPageHeight = useCallback((page: number, height: number) => {
        sizeMap.current.set(page, height);
        messageListRef.current && messageListRef.current.resetAfterIndex(0, true);
    }, [messageListRef]);
    const getPageHeight = useCallback((page: number) => {
        if (!meta?.PageCount) {
            return 300;
        }

        return sizeMap.current.get(meta?.PageCount - pageCount + page + 1) ?? 1440;
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
        if (scrollOffset < window.innerHeight / 2 && pageCount < meta.PageCount) {
            setPageCount(p => p + 1);
        }
    };

    return (
        <List
            ref={messageListRef}
            width={"calc(100vw - 384px)"}
            itemSize={getPageHeight}
            itemCount={pageCount}
            height={window.innerHeight - 48}
            onScroll={onScroll}
        >
            {({ index, style }) => {
                return (
                    <MessagePage
                        style={style}
                        selectedGuild={selectedGuild}
                        selectedChannel={selectedChannel}
                        page={meta?.PageCount - pageCount + index + 1}

                        setHeight={setPageHeight}
                    />
                );
            }}
        </List>
    );
};

export default MessagesList;