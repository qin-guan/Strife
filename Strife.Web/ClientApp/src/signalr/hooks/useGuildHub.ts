import * as React from "react";
import { useMst } from "../../models/root/Root";

import guildHubService from "../../signalr/GuildHubService";

const useGuildHub = (): void => {
    const {
        guildStore: { add }
    } = useMst();

    React.useEffect(() => {
        guildHubService.onGuildCreated(add);

        return () => {
            guildHubService.close();
        };
    }, [add]);
};

export default useGuildHub;