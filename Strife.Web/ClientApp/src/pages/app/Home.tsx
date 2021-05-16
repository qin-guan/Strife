import * as React from "react";
import { Flex, Box } from "@chakra-ui/react";
import { useEffect } from "react";

import useHub from "../../signalr/hooks/useHub";

import GuildsSidebar from "../../components/app/Guild/GuildsSidebar";
import GuildBanner from "../../components/app/Guild/GuildBanner";
import ChannelsSidebar from "../../components/app/Channel/ChannelsSidebar";

const Home = (): React.ReactElement => {
    useHub();
    
    return (
        <Flex w="100%" h="100%">
            <GuildsSidebar/>
            <Flex direction={"column"} w={80}>
                <GuildBanner/>
                <ChannelsSidebar/>
            </Flex>
        </Flex>
    );
};

export default Home;
