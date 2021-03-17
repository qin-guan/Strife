import * as React from "react";
import { observer } from "mobx-react";
import { Box, Avatar, useColorModeValue } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";
import { useMst } from "../../models/root/Root";
import { GuildInstance } from "../../models/guild/Guild";

export interface GuildsSidebarProps {
    onClickCreateGuild: () => void
}

const GuildsSidebar = (props: GuildsSidebarProps) => {
    const { onClickCreateGuild } = props;
    const bg = useColorModeValue("gray.200", "gray.900");
    const {
        guildStore: { fetchGuilds, guilds }
    } = useMst();

    React.useEffect(() => {
        fetchGuilds();
    }, [fetchGuilds]);

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

                    onClick={onClickCreateGuild}
                />
            </Box>
        </Box>
    );
};

export default observer(GuildsSidebar);