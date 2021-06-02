import * as React from "react";
import { Box, Heading } from "@chakra-ui/react";
import { useGuild } from "../../../api/swr/guilds";

export interface GuildBannerProps {
    selectedGuild: string;
}

const GuildBanner = (props: GuildBannerProps): Nullable<React.ReactElement> => {
    const { data: guild, error } = useGuild(props.selectedGuild);

    if (!guild || error) return null;

    return (
        <Box px={4} py={2}>
            <Heading size={"md"}>{guild.Name}</Heading>
        </Box>
    );
};

export default GuildBanner;
