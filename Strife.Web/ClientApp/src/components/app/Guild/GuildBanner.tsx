import * as React from "react";
import { Box, Heading, useColorModeValue } from "@chakra-ui/react";
import { useAppSelector } from "../../../store/hooks/useAppSelector";
import { currentGuild } from "../../../models/guild/GuildSlice";

const GuildBanner = (): Nullable<React.ReactElement> => {
    const guildSlice = useAppSelector(s => s.guild);
    const guild = currentGuild(guildSlice);

    if (!guild) return null;

    return (
        <Box px={4} py={2}>
            <Heading size={"md"}>{guild.Name}</Heading>
        </Box>
    );
};

export default GuildBanner;
