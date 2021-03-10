import * as React from "react";
import { useMst } from "../../models/root/Root";

import guildHubService from "../../signalr/GuildHubService";

const useGuildHub = (): void => {
    const {
        guildStore: { addGuild }
    } = useMst();

    React.useEffect(() => {
        guildHubService.onGuildCreated((guild) => {
            addGuild(guild);
        });

        return () => {
            guildHubService.close();

            console.log("unmounted");
        };
    }, [addGuild]);
};

export default useGuildHub;