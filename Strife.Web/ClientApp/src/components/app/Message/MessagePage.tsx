import * as React from "react";
import { CSSProperties, memo } from "react";

import { Box, Flex } from "@chakra-ui/react";
import { areEqual } from "react-window";

import { useMessages } from "../../../api/swr/messages";

import Message from "./Message";

export interface MessagePageProps {
    selectedGuild: string;
    selectedChannel: string;
    style: CSSProperties;
    page: number;
}

const MessagePage = (props: MessagePageProps) => {
    const { selectedGuild, selectedChannel, page, style } = props;
    const { data, error } = useMessages(selectedGuild, selectedChannel, page);

    if (!data) {
        return <>Loading...</>;
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