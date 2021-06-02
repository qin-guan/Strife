import { apiClient } from "./base";
import { Message } from "../../models/Message";

export interface ReadMessagesResponseDto {
    Messages: Message[];
    HasNextPage: boolean;
    HasPreviousPage: boolean;
    PageIndex: number;
    TotalPages: number;
}

export interface MessagesApi {
    get: (page: number) => Promise<Message[]>;
    swrGet: (url: string) => Promise<Message[]>;
    meta: () => Promise<{Pages: number; PageSize: number; Count: number}>;
}

const messages = (guildId: string, channelId: string): MessagesApi => ({
    get: async (page: number) => {
        return (await apiClient.get(`Guilds/${guildId}/Channels/${channelId}/Messages`, {
            searchParams: {
                page: page,
            }
        }).json<ReadMessagesResponseDto>()).Messages;
    },
    swrGet: async (url: string) => {
        const msgs: Message[] = (await apiClient.get(url).json<ReadMessagesResponseDto>()).Messages;
        return msgs;
    },
    meta: async () => {
        return await apiClient.get(`Guilds/${guildId}/Channels/${channelId}/Messages/Meta`).json();
    }
});

export default messages;
