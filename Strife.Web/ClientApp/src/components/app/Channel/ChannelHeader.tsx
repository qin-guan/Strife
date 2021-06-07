import * as React from "react";
import { FC } from "react";
import { Box, Flex, Heading, Text, useColorModeValue } from "@chakra-ui/react";
import { useChannel } from "../../../api/swr/channels";

export interface ChannelHeaderProps {
    selectedGuild: string;
    selectedChannel: string;
}

export const ChannelHeader: FC<ChannelHeaderProps> = (props) => {
    const { selectedGuild, selectedChannel } = props;
    const { data, error } = useChannel(selectedGuild, selectedChannel);
    
    const bg = useColorModeValue("gray.200", "gray.700");

    if (!data) {
        return (
            <Box>
                Loading...
            </Box>
        );
    }

    return (
        <Flex p={3} bg={bg} borderRadius={"lg"}>
            <Heading size={"md"} color={"gray.400"}>
                #
            </Heading>
            <Heading ml={2} size={"md"} color={"white"}>
                {data.Name}
            </Heading>
        </Flex>
    );
};