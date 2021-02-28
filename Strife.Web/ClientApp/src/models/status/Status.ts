import { types } from "mobx-state-tree"

export const Status = types.enumeration("Status", ["loading", "error", "done", "empty"])
