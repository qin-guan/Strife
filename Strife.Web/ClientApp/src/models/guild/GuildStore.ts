import { flow, types } from "mobx-state-tree";

import { Guild } from "./Guild";
import { Status } from "../status/Status"

import { guild as guildApi } from "../../api/http/Guild";

export const GuildStore = types
    .model({
        guilds: types.array(Guild),
        status: types.optional(Status, "empty"),
    })
    .actions((self) => ({
        fetchGuilds: flow(function* () {
            self.status = "loading"
            try {
                const data = yield guildApi.get();

                self.guilds = data
                self.status = "done"
            } catch (error) {
                console.error("Failed to load guilds", error)
                self.status = "error"
            } 
        }),
    }));
