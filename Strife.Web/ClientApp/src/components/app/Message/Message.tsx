import * as React from "react";
import { FC, memo } from "react";
import { Avatar, Box, Flex, Tag, TagLabel, Text, useColorModeValue } from "@chakra-ui/react";
import { useUser } from "../../../api/swr/users";
import { areEqual } from "react-window";

import format from "date-fns/format";

export interface MessageProps {
    content: string;
    dateSent: Date;
    user: string;
}

const Message: FC<MessageProps> = (props) => {
    const { content, dateSent, user } = props;
    const { data: userData, error } = useUser(user);

    const hoverBg = useColorModeValue("gray.200", "gray.900");

    if (!userData) {
        return (
            <Flex>Loading...</Flex>
        );
    }

    return (
        <Flex p={2} alignItems={"center"} justifyContent={"space-between"} _hover={{ backgroundColor: hoverBg }} borderRadius={"md"}>
            <Flex alignItems={"center"}>
                <Tag size="lg" colorScheme="red" borderRadius="full" mr={3}>
                    <Avatar
                        size="xs"
                        name={userData.DisplayName}
                        ml={-1}
                        mr={2}
                    />
                    <TagLabel>{userData.DisplayName}</TagLabel>
                </Tag>
                <Text style={{ wordWrap: "break-word", maxWidth: "75%" }}>{content}</Text>
            </Flex>
            <Text color={"gray.400"}>{format(new Date(dateSent), "HH:mm MM/dd/yyyy")}</Text>
        </Flex>
    );
};

export default memo(Message, areEqual);