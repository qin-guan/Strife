import * as React from "react";
import { ChangeEventHandler, FC, KeyboardEventHandler, useState } from "react";

import { Box, Input } from "@chakra-ui/react";
import messages from "../../../api/http/messages";

export interface MessageInputProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const MessageInput: FC<MessageInputProps> = (props) => {
    const { selectedGuild, selectedChannel } = props;
    const [message, setMessage] = useState("");

    const { create } = messages(selectedGuild, selectedChannel);

    const onKeyPress: KeyboardEventHandler<HTMLInputElement> = async (event) => {
        if (event.code !== "Enter" || message.length === 0) {
            return;
        }

        create(message);
        setMessage("");
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
};