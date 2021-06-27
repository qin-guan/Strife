import * as React from "react";
import { Flex, Heading, IconButton } from "@chakra-ui/react";
import { useGuild } from "../../../api/swr/guilds";
import { SettingsIcon } from "@chakra-ui/icons";
import { useState } from "react";
import GuildSettingsModal from "./GuildSettingsModal";

export interface GuildBannerProps {
    selectedGuild: string;
}

const GuildBanner = (props: GuildBannerProps): Nullable<React.ReactElement> => {
    const { data: guild, error } = useGuild(props.selectedGuild);
    
    const [settingsOpen, setSettingsOpen] = useState(false);

    if (!guild || error) return null;
    
    const closeSettings = () => setSettingsOpen(false);
    const openSettings = () => setSettingsOpen(true);

    return (
        <Flex px={4} py={2} justifyContent={"space-between"} alignItems={"center"}>
            <GuildSettingsModal isOpen={settingsOpen} onClose={closeSettings}/>
            <Heading size={"md"}>{guild.Name}</Heading>
            <IconButton aria-label={"settings"} icon={<SettingsIcon/>} onClick={openSettings}/>
        </Flex>
    );
};

export default GuildBanner;
