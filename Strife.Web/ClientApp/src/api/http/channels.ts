import { apiClient } from "./base";
import { CreateChannelRequest } from "../models/channels/CreateChannelRequest";
import { Channel } from "../../models/channel/Channel";

export interface ChannelsApi {
    get: () => Promise<Channel[]>;
    find: (channelId: string) => Promise<Channel>;
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
    }
});

export default channels;
