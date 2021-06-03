import * as React from "react";
import { FC, memo } from "react";
import { Avatar, Box, Flex, Tag, TagLabel, Text, useColorModeValue } from "@chakra-ui/react";
import { useUser } from "../../../api/swr/users";
import { areEqual } from "react-window";

export interface MessageProps {
    content: string;
    user: string;
}

const Message: FC<MessageProps> = (props) => {
    const { content, user } = props;
    const { data: userData, error } = useUser(user);

    const hoverBg = useColorModeValue("gray.200", "gray.900");

    if (!userData) {
        return (
            <Flex>Loading...</Flex>
        );
    }

    return (
        <Flex p={2} alignItems={"center"} _hover={{ backgroundColor: hoverBg }} borderRadius={"md"}>
            <Tag size="lg" colorScheme="red" borderRadius="full" mr={3}>
                <Avatar
                    size="xs"
                    name={userData.DisplayName}
                    ml={-1}
                    mr={2}
                />
                <TagLabel>{userData.DisplayName}</TagLabel>
            </Tag>
            {content}
        </Flex>
    );
};

export default memo(Message, areEqual);