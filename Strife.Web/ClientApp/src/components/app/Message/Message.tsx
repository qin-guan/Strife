import * as React from "react";
import { FC, memo } from "react";
import { Avatar, Flex, Tag, Text, useColorModeValue } from "@chakra-ui/react";
import { useUser } from "../../../api/swr/users";

import { areEqual } from "react-window";
import { formatRelative } from "date-fns";

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
        <Flex p={2} py={3} _hover={{ backgroundColor: hoverBg }} borderRadius={"md"}>
            <Flex alignItems={"flex-start"}>
                <Avatar
                    size={"md"}
                    name={userData.DisplayName}
                    mr={4}
                />
            </Flex>
            <Flex wordBreak={"break-word"} style={{ flex: 1 }} flexDirection={"column"} justifyContent={"center"} alignItems={"flex-start"}>
                <Flex mb={1}>
                    <Tag size={"md"}>{userData.DisplayName}</Tag>
                    <Text ml={3} color={"gray.400"}>{formatRelative(new Date(dateSent), new Date())}</Text>
                </Flex>
                <Text color={"gray.100"}>{content}</Text>
            </Flex>
        </Flex>
    );
};

export default memo(Message, areEqual);