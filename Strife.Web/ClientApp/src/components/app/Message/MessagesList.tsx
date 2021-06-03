import * as React from "react";
import { createRef, FC, ReactElement, useEffect, useRef, useState } from "react";

import { Center, Flex, Heading, Spinner, Tag, useToast } from "@chakra-ui/react";
import { VariableSizeList, VariableSizeList as List } from "react-window";

import { useChannelMeta } from "../../../api/swr/channels";

import MessagePage from "./MessagePage";

export interface MessagesListProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const MessagesList: FC<MessagesListProps> = (props) => {
    const { selectedGuild, selectedChannel } = props;
    const { data: meta, error } = useChannelMeta(selectedGuild, selectedChannel);
    const [pageCount, setPageCount] = useState(2);
    const messageListRef = createRef<VariableSizeList>();

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

    const getItemSize = (index: number) => index === 0 ? 300 : 1440;
    const onScroll = ({ scrollOffset }: { scrollOffset: number }) => {
        if (scrollOffset < 300 && pageCount < meta.PageCount) {
            setPageCount(p => p + 1);
            messageListRef.current && messageListRef.current.scrollToItem(2, "start");
        }
    };

    return (
        <List
            ref={messageListRef}
            width={"calc(100vw - 384px)"}
            itemSize={getItemSize}
            itemCount={pageCount + 1}
            height={window.innerHeight}
            onScroll={onScroll}
        >
            {({ index, style }) => {
                if (index === 0) {
                    return (
                        <Center style={{ height: 300 }}>
                            <Tag size={"lg"} variant="solid" colorScheme="teal">
                                No more messages :(
                            </Tag>
                        </Center>
                    );
                }
                return (
                    <MessagePage style={style} selectedGuild={selectedGuild}
                        selectedChannel={selectedChannel} page={pageCount - index}/>
                );
            }}
        </List>
    );
};

export default MessagesList;