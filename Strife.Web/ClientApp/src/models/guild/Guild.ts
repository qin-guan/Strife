import { types } from "mobx-state-tree"

export const Guild = types.model({
    id: types.string,
    name: types.string,
})
