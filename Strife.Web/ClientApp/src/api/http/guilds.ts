import { apiClient } from "./base";

import { CreateGuildRequest } from "../models/guilds/CreateGuildRequest";
import { Guild } from "../../models/guild/Guild";

const get = async (): Promise<Guild[]> => {
    return await apiClient.get("Guilds").json();
};

const find = async (id: string): Promise<Guild> => {
    return await apiClient.get(`Guilds/${id}`).json();
};

const create = async (guild: CreateGuildRequest): Promise<Guild> => {
    return await apiClient.post("Guilds", { json: guild }).json();
};

const subscribe = async (id: string, connectionId: string): Promise<void> => {
    await apiClient.post(`Guilds/${id}/Subscribe`, { json: { ConnectionId: connectionId } });
};

const guilds = {
    get,
    find,
    create,
    subscribe,
};

export default guilds;
