import { flow, types } from "mobx-state-tree";

import { Guild, GuildInstance } from "./Guild";
import { Status } from "../status/Status";

import { get } from "../../api/http/Guilds";

const GuildStore = types
    .model({
        guilds: types.array(Guild),

        guildSidebarStatus: types.optional(Status, "empty")
    })
    .actions((self) => ({
        fetchGuilds: flow(function* () {
            self.guildSidebarStatus = "loading";
            try {
                const data = yield get();

                self.guilds = data;
                self.guildSidebarStatus = "done";
            } catch (error) {
                console.error("Failed to load guilds", error);
                self.guildSidebarStatus = "error";
            } 
        }),

        add: function (guild: GuildInstance) {
            self.guilds.push(guild);
        },
    }));

export default GuildStore;