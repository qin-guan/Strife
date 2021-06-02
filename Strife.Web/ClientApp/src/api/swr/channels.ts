import useSWR from "swr";
import channelsApi from "../http/channels";
import { Channel } from "../../models/Channel";

export const useChannels = (guildId: string) => {
    const { get } = channelsApi(guildId);
    return useSWR<Channel[]>(`Guilds/${guildId}/Channels`, get);
};