import { Instance, types } from "mobx-state-tree";

export const User = types.model({
    Id: types.string,
    DisplayName: types.string,
});

export type UserInstance = Instance<typeof User>

export default User;