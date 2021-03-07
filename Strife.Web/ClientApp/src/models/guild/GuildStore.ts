import { flow, types } from "mobx-state-tree";

import { Guild, IGuild } from "./Guild";
import { Status } from "../status/Status"

import { guilds as guildApi } from "../../api/http/Guilds";

export const GuildStore = types
    .model({
        guilds: types.array(Guild),
        createGuildModalOpen: types.boolean,

        guildSidebarStatus: types.optional(Status, "empty")
    })
    .actions((self) => ({
        fetchGuilds: flow(function* () {
            self.guildSidebarStatus = "loading"
            try {
                const data = yield guildApi.get();

                self.guilds = data
                self.guildSidebarStatus = "done"
            } catch (error) {
                console.error("Failed to load guilds", error)
                self.guildSidebarStatus = "error"
            } 
        }),

        addGuild: function ({ guild }: {guild: IGuild}) {
            self.guilds.push(guild)
        },

        openCreateGuildModal: function () {
            self.createGuildModalOpen = true;
        },
        closeCreateGuildModal: function () {
            self.createGuildModalOpen = false;
        }
    }));
