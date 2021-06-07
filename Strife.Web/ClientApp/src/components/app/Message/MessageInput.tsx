import * as React from "react";
import { ChangeEventHandler, FC, KeyboardEventHandler, useState } from "react";

import { Box, Input } from "@chakra-ui/react";
import messages from "../../../api/http/messages";
import { useChannelMeta } from "../../../api/swr/channels";
import { useMessages } from "../../../api/swr/messages";

export interface MessageInputProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const MessageInput: FC<MessageInputProps> = (props) => {
    const { selectedGuild, selectedChannel } = props;
    const [message, setMessage] = useState("");
    const { data: meta, error } = useChannelMeta(selectedGuild, selectedChannel);
    const { mutate } = useMessages(selectedGuild, selectedChannel, meta ? meta.PageCount : 1);

    if (!meta) {
        return <>Loading</>;
    }

    if (error) {
        return <>error</>;
    }

    const { create } = messages(selectedGuild, selectedChannel);

    const onKeyPress: KeyboardEventHandler<HTMLInputElement> = async (event) => {
        if (event.code !== "Enter") {
            return;
        }
        await create(message);
        setMessage("");
        mutate();
    };

    const onChange: ChangeEventHandler<HTMLInputElement> = (event) => {
        setMessage(event.currentTarget.value);
    };

    return (
        <Box p={3}>
            <Input
                bg={"gray.700"}
                size={"lg"}
                variant="filled"
                placeholder="Send a message..."
                value={message}
                onKeyPress={onKeyPress}
                onChange={onChange}
            />
        </Box>
    );
}
;