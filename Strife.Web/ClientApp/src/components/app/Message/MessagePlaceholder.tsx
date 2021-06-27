import * as React from "react";
import { memo, ReactElement } from "react";
import { Avatar, Flex, Tag, TagLabel } from "@chakra-ui/react";

export default memo(function MessagePlaceholder(): ReactElement {
    return (
        <Flex p={2} alignItems={"center"} justifyContent={"space-between"} borderRadius={"md"}>
            <Flex alignItems={"center"}>
                <Tag size="lg" colorScheme="red" borderRadius="full" mr={3}>
                    <Avatar
                        size="xs"
                        name={""}
                        ml={-1}
                        mr={2}
                    />
                    <TagLabel>...</TagLabel>
                </Tag>
            </Flex>
        </Flex>
    );
});