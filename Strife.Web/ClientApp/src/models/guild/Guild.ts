import { types, Instance } from "mobx-state-tree";

export const Guild = types.model({
    Id: types.string,
    Name: types.string,
    Channels: types.array(types.string),
    Roles: types.array(types.string),
    Users: types.array(types.string)
});

export type GuildInstance = Instance<typeof Guild>

export default Guild;