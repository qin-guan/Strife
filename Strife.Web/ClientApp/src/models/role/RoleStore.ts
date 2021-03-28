import { flow, types } from "mobx-state-tree";

import { Role, RoleInstance } from "./Role";

const RoleStore = types
    .model({
        roles: types.array(Role),
    })
    .actions((self) => ({
    }));

export default RoleStore;