import { apiClient } from "./base";
import { Message } from "../../models/message/Message";

export interface MessagesApi {
    get: () => Promise<Message[]>;
}

const messages = (guildId: string, channelId: string): MessagesApi => ({
    get: async () => {
        return await apiClient.get(`Guilds/${guildId}/Channels/${channelId}/Messages`).json<Message[]>();
    },
});

export default messages;
