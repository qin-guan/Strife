import { types } from "mobx-state-tree";

import User, { UserInstance } from "./User";

const UserStore = types
    .model({
        users: types.array(User),
    })
    .actions((self) => ({}));

export default UserStore;
