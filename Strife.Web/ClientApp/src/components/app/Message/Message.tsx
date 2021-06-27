import * as React from "react";
import { FC, memo } from "react";
import { Avatar, Flex, Tag, TagLabel, Text, useColorModeValue } from "@chakra-ui/react";
import { useUser } from "../../../api/swr/users";

import format from "date-fns/format";
import { areEqual } from "react-window";

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
        <Flex p={2} _hover={{ backgroundColor: hoverBg }} borderRadius={"md"}>
            <Flex alignItems={"flex-start"}>
                <Tag size="lg" colorScheme="red" borderRadius="full" mr={3}>
                    <Avatar
                        size="xs"
                        name={userData.DisplayName}
                        ml={-1}
                        mr={2}
                    />
                    <TagLabel>{userData.DisplayName}</TagLabel>
                </Tag>
            </Flex>
            <Flex wordBreak={"break-word"} style={{ flex: 1 }}>
                {content}
            </Flex>
            <Flex>
                <Text color={"gray.400"}>{format(new Date(dateSent), "HH:mm MM/dd/yyyy")}</Text>
            </Flex>
        </Flex>
    );
};

export default memo(Message, areEqual);