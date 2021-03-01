import * as React from "react";
import { observer } from "mobx-react";
import { Box, useColorModeValue } from "@chakra-ui/react";
import { useMst } from "../../models/root/Root";

export const GuildSidebar = observer(() => {
    const bg = useColorModeValue("gray.200", "gray.900");
    const {
        guildStore: { fetchGuilds, guilds, status },
    } = useMst();

    React.useEffect(() => {
        fetchGuilds();
    }, [fetchGuilds]);

    return (
        <Box maxW={16} w={16} bg={bg}>
            {JSON.stringify(guilds)}
            {status}
        </Box>
    );
});
