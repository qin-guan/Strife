import { apiClient } from "./Base";

import { GuildInstance } from "../../models/guild/Guild";

export const get = async (): Promise<GuildInstance> => {
    return await apiClient.get("Guilds").json();
};

export const find = async (id: string): Promise<GuildInstance> => {
    return await apiClient.get(`Guilds/${id}`).json();
};

export const add = async (guild: GuildInstance): Promise<GuildInstance> => {
    return await apiClient.post("Guilds", { json: guild }).json();
};

export const subscribe = async (id: string, connectionId: string): Promise<void> => {
    await apiClient.post(`Guilds/${id}/Subscribe`, { json: { ConnectionId: connectionId } });
    return;
};

const guilds = {
    get,
    find,
    add,
    subscribe
};

export default guilds;