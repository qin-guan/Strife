import { apiClient } from "./Base";

export const guild = {
    get: async () => {
        return await apiClient.get("/guild/guilds");
    },
}
