import { hostnames } from "../api/http/Base";
import * as signalR from "@microsoft/signalr";

import authorizationService from "../oidc/AuthorizationService";

import { GuildInstance } from "../models/guild/Guild";
import { find, subscribe } from "../api/http/Guilds";

import { UserInstance } from "../models/user/User";
import { RoleInstance } from "../models/role/Role";


export const GuildEvents = {
    GuildCreated: "Guild/Created",
    GuildUserAdded: "Guild/UserAdded",
    GuildRoleCreated: "Guild/RoleCreated"
};

export class GuildHubService {
    _connection?: signalR.HubConnection;

    async ensureConnectionExists(): Promise<void> {
        if (this._connection !== undefined) return;
        this._connection = new signalR.HubConnectionBuilder()
            .withUrl(`${hostnames.api}/hubs/guild`, {
                accessTokenFactory: async () => await authorizationService.getAccessToken(),
            })
            .build();
        try {
            await this._connection.start();
        } catch (error) {
            console.error(error);
            throw error;
        }
    }

    async getConnection(): Promise<signalR.HubConnection> {
        await this.ensureConnectionExists();

        if (!this._connection) throw new Error("Connection does not exist");

        return this._connection;
    }

    async onGuildCreated(callback: (guild: GuildInstance) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(GuildEvents.GuildCreated, async (id: string) => {
            if (!connection.connectionId) throw new Error("Connection does not exist");
            const [_, guild] = await Promise.all([subscribe(id, connection.connectionId), find(id)]);
            callback(guild);
        });
    }

    async onGuildUserAdded(callback: (user: UserInstance) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(GuildEvents.GuildUserAdded, async (userId: string) => {
            console.log(`${userId} joined guild`);
        });
    }
    
    async onGuildRoleCreated(callback: (role: RoleInstance) => void): Promise<void> {
        const connection = await this.getConnection();
        
        connection.on(GuildEvents.GuildRoleCreated, async (roleName: string) => {
            console.log(`${roleName} created in guild`);
        });
    }

    async close(): Promise<void> {
        const connection = await this.getConnection();
        await connection.stop();
        this._connection = undefined;
    }

    static get instance(): GuildHubService {
        return guildHubService;
    }
}

const guildHubService = new GuildHubService();

export default guildHubService;
