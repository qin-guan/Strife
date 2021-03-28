import { types, Instance } from "mobx-state-tree";

export const Role = types.model({
    Name: types.string,
    Policies: types.array(types.string)
});

export type RoleInstance = Instance<typeof Role>

export default Role;