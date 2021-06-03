import useSWR from "swr";

import { fetcher } from "./fetcher";
import { Message } from "../../models/Message";

export const useMessages = (guildId: string, channelId: string, page: number) => {
    return useSWR<Message[]>(`Guilds/${guildId}/Channels/${channelId}/Messages?page=${page}`, fetcher, {
        revalidateOnFocus: false,
    });
};