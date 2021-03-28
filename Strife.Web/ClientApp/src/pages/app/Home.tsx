import * as React from "react";
import { useEffect } from "react";
import { Flex } from "@chakra-ui/react";
import { observer } from "mobx-react";

import { useMst } from "../../models/root/Root";
import guildsApi from "../../api/http/Guilds";
import guildHubService from "../../signalr/GuildHubService";

import GuildsSidebar from "../../components/app/GuildsSidebar";
import CreateGuildModal from "../../components/app/CreateGuildModal";
import useGuildHub from "../../signalr/hooks/useGuildHub";

const Home = (): React.ReactElement => {
    const {
        guildStore: { fetch, guilds }
    } = useMst();

    const [createGuildModalOpen, setCreateGuildModalOpen] = React.useState(false);

    const onCloseCreateGuildModal = (): void => {
        setCreateGuildModalOpen(false);
    };

    const onClickCreateGuild = (): void => {
        setCreateGuildModalOpen(true);
    };

    useEffect(() => {
        fetch();
        const subscribeToAllGuilds = async () => {
            const { connectionId } = await guildHubService.getConnection();
            if (!connectionId) throw new Error("Connection does not exist");
            await Promise.all(guilds.map((guild) => guildsApi.subscribe(guild.Id, connectionId)));
        };
        subscribeToAllGuilds();
    }, [fetch, guilds]);
    
    useGuildHub();
    
    return (
        <Flex w="100%" h="100%">
            <CreateGuildModal 
                isOpen={createGuildModalOpen}
                onClose={onCloseCreateGuildModal}
            />
            <GuildsSidebar 
                onClickCreateGuild={onClickCreateGuild}
            />
        </Flex>
    );
};

export default observer(Home);