import * as React from "react";
import { Flex } from "@chakra-ui/react";

import GuildsSidebar from "../../components/app/GuildsSidebar";
import CreateGuildModal from "../../components/app/CreateGuildModal";
import useGuildHub from "../../signalr/hooks/useGuildHub";

const Home = (): React.ReactElement => {
    useGuildHub();
    return (
        <Flex w="100%" h="100%">
            <CreateGuildModal/>
            <GuildsSidebar/>
        </Flex>
    );
};

export default Home;