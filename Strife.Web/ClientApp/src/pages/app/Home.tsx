import * as React from "react";
import { Flex } from "@chakra-ui/react";

import GuildsSidebar from "../../components/app/GuildsSidebar";
import CreateGuildModal from "../../components/app/CreateGuildModal";
import useGuildHub from "../../signalr/hooks/useGuildHub";

const Home = (): React.ReactElement => {
    const [createGuildModalOpen, setCreateGuildModalOpen] = React.useState(false);

    const onCloseCreateGuildModal = (): void => {
        setCreateGuildModalOpen(false);
    };

    const onClickCreateGuild = (): void => {
        setCreateGuildModalOpen(true);
    };

    useGuildHub();
    return (
        <Flex w="100%" h="100%">
            <CreateGuildModal isOpen={createGuildModalOpen} onClose={onCloseCreateGuildModal}/>
            <GuildsSidebar onClickCreateGuild={onClickCreateGuild}/>
        </Flex>
    );
};

export default Home;