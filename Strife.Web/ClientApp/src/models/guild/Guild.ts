import { types, Instance } from "mobx-state-tree";

export const Guild = types.model({
    Id: types.string,
    Name: types.string,
});

export type GuildInstance = Instance<typeof Guild>

export default Guild;