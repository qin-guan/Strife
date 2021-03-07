import { apiClient } from "./Base";

import { IGuild } from "../../models/guild/Guild"

export const guilds = {
    get: async () => {
        return await apiClient.get("Guilds").json();
    },
    add: async (guild: IGuild) => {
        return await apiClient.post("Guilds", { json: guild }).json();
    }
}
