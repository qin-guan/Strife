import { apiClient } from "./base";

export interface RolesApi {
    subscribe: (connectionId: string) => void;
}

const roles = (guildId: string): RolesApi => ({
    subscribe: async (connectionId: string) => {
        await apiClient.post(`Guilds/${guildId}/Roles/${guildId}/Subscribe`, { json: { ConnectionId: connectionId } });
    }
});

export default roles;
