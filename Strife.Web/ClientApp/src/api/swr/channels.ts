import useSWR from "swr";
import channelsApi from "../http/channels";
import { Channel } from "../../models/Channel";
import { fetcher } from "./fetcher";
import { ChannelMetaResponseDto } from "../dtos/channels/ChannelMetaResponseDto";

export const useChannels = (guildId: string) => {
    const { get } = channelsApi(guildId);
    return useSWR<Channel[]>(`Guilds/${guildId}/Channels`, get);
};

export const useChannel = (guildId: string, channelId: string) => {
    return useSWR<Channel>(`Guilds/${guildId}/Channels/${channelId}`, fetcher);
};

export const useChannelMeta = (guildId: string, channelId: string) => {
    return useSWR<ChannelMetaResponseDto>(`Guilds/${guildId}/Channels/${channelId}/Meta`, fetcher);
};
