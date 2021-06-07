import useSWR from "swr";
import guildsApi from "../http/guilds";
import { Guild } from "../../models/Guild";
import { fetcher } from "./fetcher";

export const useGuilds = () => {
    return useSWR<Guild[]>("Guilds", guildsApi.get);
}; 

export const useGuild = (guildId: string) => {
    return useSWR<Guild>(`Guilds/${guildId}`, fetcher);
};