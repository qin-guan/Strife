/** @jsxImportSource @emotion/react */
import { css, jsx } from "@emotion/react";
import { observer } from "mobx-react";

import * as React from "react";
import { Box, Avatar, useColorModeValue } from "@chakra-ui/react";
import { AddIcon } from "@chakra-ui/icons";
import { GuildInstance } from "../../models/guild/Guild";
import { useMst } from "../../models/root/Root";

export interface GuildsSidebarProps {
    onClickCreateGuild: () => void,
}

const GuildsSidebar = (props: GuildsSidebarProps) => {
    const { guildStore: { guilds } } = useMst();
    const { onClickCreateGuild } = props;
    const bg = useColorModeValue("gray.200", "gray.900");
    return (
        <Box bg={bg} px={2} css={css`
            overflow-y: scroll;
            -ms-overflow-style: none; 
            scrollbar-width: none;
            &::-webkit-scrollbar {
                display: none;
            }
        `}>
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