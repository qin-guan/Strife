import * as React from "react";

import { useCallback, useEffect } from "react";
import hubService from "../HubService";
import { useAppDispatch } from "../../store/hooks/useAppDispatch";
import { add as addGuild } from "../../models/guild/GuildSlice";
import { addToGuild as addChannel } from "../../models/channel/ChannelSlice";
import { add as addRole } from "../../models/role/RoleSlice";

const useHub = (): void => {
    const dispatch = useAppDispatch();

    const addHandlers = useCallback(async () => {
        await hubService.onGuildCreated(guild => dispatch(addGuild(guild)));
        await hubService.onRoleCreated(role => dispatch(addRole(role)));
        await hubService.onChannelCreated((guildId, channel) => dispatch(addChannel({ channel, guildId })));
    }, [dispatch]);

    const start = useCallback(async () => {
        await hubService.start();
    }, []);

    const stop = useCallback(async () => {
        await hubService.close();
    }, []);

    useEffect(() => {
        start();
        
        return () => {
            stop();
        };
    }, [start, stop]);

    useEffect(() => {
        addHandlers();
    }, [addHandlers]);
};

export default useHub;
