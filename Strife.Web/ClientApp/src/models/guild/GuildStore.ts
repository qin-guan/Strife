import {flow, types} from "mobx-state-tree"

import {Guild} from "./Guild";

export const GuildStore = types
    .model({
        guilds: types.map(Guild),
    })
    .actions(self => ({
        loadGuilds: flow(function * () {
            
        })
    }))
