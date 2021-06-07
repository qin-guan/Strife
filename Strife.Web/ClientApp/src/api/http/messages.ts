import { apiClient } from "./base";

const messages = (guildId: string, channelId: string) => ({
    create: async (message: string) => {
        return apiClient.post(`Guilds/${guildId}/Channels/${channelId}/Messages`, { json: { Content: message } });
    }
});

export default messages;
