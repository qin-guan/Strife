import * as React from "react";
import { observer } from "mobx-react";
import { Box, Avatar, useColorModeValue } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";
import { useMst } from "../../models/root/Root";
import { GuildInstance } from "../../models/guild/Guild";

const GuildsSidebar = observer(() => {
    const bg = useColorModeValue("gray.200", "gray.900");
    const {
        guildStore: { fetchGuilds, openCreateGuildModal, guilds }
    } = useMst();

    React.useEffect(() => {
        fetchGuilds();
    }, [fetchGuilds]);

    const onClickCreateGuildAvatar = () => {
        openCreateGuildModal();
    };

    return (
        <Box bg={bg} px={2} style={{ overflowY: "scroll" }}>
            {guilds.map((guild: GuildInstance, idx: number) => (
                <Box py={2} key={idx.toString()}>
                    <Avatar
                        size="md"
                        name={guild.Name}
                        style={{ cursor: "pointer" }}

                    />
                </Box>
            ))}
            <Box py={2}>
                <Avatar
                    size="md"
                    icon={<AddIcon/>}
                    style={{ cursor: "pointer" }}

                    onClick={onClickCreateGuildAvatar}
                />
            </Box>
        </Box>
    );
});

export default GuildsSidebar;