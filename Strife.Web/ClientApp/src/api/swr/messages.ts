import useSWR from "swr";
import messagesApi from "../http/messages";

export const useMessagesMeta = (guildId: string, channelId: string) => {
    const { meta } = messagesApi(guildId, channelId);
    return useSWR(`Guilds/${guildId}/Channels/${channelId}/Messages/Meta`, meta);
};

export const useMessages = (guildId: string, channelId: string, page: number) => {
    const { swrGet } = messagesApi(guildId, channelId);

    return useSWR(`Guilds/${guildId}/Channels/${channelId}/Messages?page=${page}`, swrGet, {
        revalidateOnFocus: false,
    });
};