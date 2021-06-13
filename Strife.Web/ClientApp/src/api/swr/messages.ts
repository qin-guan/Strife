import useSWR from "swr";

import { fetcher } from "./fetcher";
import { Message } from "../../models/Message";

export const useMessages = (guildId: string, channelId: string, page: Nullable<number>) => {
    return useSWR<Message[]>(`Guilds/${guildId}/Channels/${channelId}/Messages?page=${page}`, page === null ? null : fetcher, {
        revalidateOnFocus: false,
    });
};