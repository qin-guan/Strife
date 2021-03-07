import { apiClient } from "./Base";

import { IGuild } from "../../models/guild/Guild"

export const guilds = {
    find: async ({ id }: {id: string}): Promise<IGuild> => {
        return await apiClient.get(`Guilds/${id}`).json()
    },
    get: async (): Promise<IGuild> => {
        return await apiClient.get("Guilds").json();
    },
    add: async ({ guild }: {guild: IGuild}): Promise<IGuild> => {
        return await apiClient.post("Guilds", { json: guild }).json();
    },
}
