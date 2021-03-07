import * as React from "react";
import { observer } from "mobx-react";
import { Box, Avatar, useColorModeValue } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";
import { useMst } from "../../models/root/Root";

export const GuildsSidebar = observer(() => {
    const bg = useColorModeValue("gray.200", "gray.900");
    const {
        guildStore: { fetchGuilds, openCreateGuildModal, guilds }
    } = useMst();

    React.useEffect(() => {
        fetchGuilds();
    }, [fetchGuilds]);

    const onClickCreateGuildAvatar = () => {
        openCreateGuildModal();
    }

    return (
        <Box bg={bg} px={2}>
            {guilds.map((guild) => (
                <Box py={2}>
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
