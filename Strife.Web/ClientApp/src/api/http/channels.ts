import { apiClient } from "./base";
import { CreateChannelRequest } from "../dtos/channels/CreateChannelRequest";
import { Channel } from "../../models/Channel";

import { ChannelMetaResponseDto } from "../dtos/channels/ChannelMetaResponseDto";

export interface ChannelsApi {
    get: () => Promise<Channel[]>;
    find: (channelId: string) => Promise<Channel>;
    meta: (channelId: string) => Promise<ChannelMetaResponseDto>;
    create: (json: CreateChannelRequest) => Promise<Channel>;
}

const channels = (guildId: string): ChannelsApi => ({
    get: async () => {
        return await apiClient.get(`Guilds/${guildId}/Channels`).json<Channel[]>();
    },
    find: async (channelId: string) => {
        return await apiClient.get(`Guilds/${guildId}/Channels/${channelId}`).json<Channel>();
    },
    create: async (json: CreateChannelRequest) => {
        return await apiClient.post(`Guilds/${guildId}/Channels`, { json }).json<Channel>();
    },
    meta: async (channelId: string) => {
        return await apiClient.get(`Guilds/${guildId}/Channels/${channelId}/Messages/Meta`).json();
    }
});

export default channels;
