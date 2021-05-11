import * as signalR from "@microsoft/signalr";

import { hostnames } from "../api/http/base";
import authorizationService from "../oidc/AuthorizationService";

import guildsApi from "../api/http/guilds";
import channelsApi from "../api/http/channels";
import { Guild } from "../models/guild/Guild";
import { User } from "../models/user/User";
import { Role } from "../models/role/Role";
import { Channel } from "../models/channel/Channel";

export const Events = {
    Guild: {
        Created: "Guilds/Created",
        UserAdded: "Guilds/UserAdded"
    },
    Role: {
        Created: "Roles/Created",
        UserAdded: "Roles/UserAdded"
    },
    Channel: {
        Created: "Channels/Created"
    }
};

export class HubService {
    _connection?: signalR.HubConnection;

    async ensureConnectionExists(): Promise<void> {
        if (this._connection !== undefined) return;
        this._connection = new signalR.HubConnectionBuilder()
            .withUrl(`${hostnames.api}/hub`, {
                accessTokenFactory: async () => await authorizationService.getAccessToken(),
            })
            .build();
        try {
            await this._connection.start();
        } catch (error) {
            console.error(error);
        }
    }
    
    async start(): Promise<void> {
        await this.ensureConnectionExists();
    }

    async getConnection(): Promise<signalR.HubConnection> {
        await this.ensureConnectionExists();

        if (!this._connection) throw new Error("Connection does not exist");

        return this._connection;
    }

    async subscribeToGuild(guildId: string): Promise<void> {
        const connection = await this.getConnection();
        if (!connection.connectionId) throw new Error("Connection does not exist");
        await guildsApi.subscribe(guildId, connection.connectionId);
    }

    async onGuildCreated(callback: (guild: Guild) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(Events.Guild.Created, async (id: string) => {
            await this.subscribeToGuild(id);
            const guild = await guildsApi.find(id);
            callback(guild);
        });
    }

    async onGuildUserAdded(callback: (user: User) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(Events.Guild.UserAdded, async (userId: string) => {
            console.log(`${userId} joined guild`);
        });
    }

    async onRoleCreated(callback: (role: Role) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(Events.Role.Created, async (roleName: string) => {
            console.log(`${roleName} created in guild`);
        });
    }

    async onChannelCreated(callback: (guildId: string, channel: Channel) => void): Promise<void> {
        const connection = await this.getConnection();

        connection.on(Events.Channel.Created, async (guildId: string, channelId: string) => {
            try {
                const channel = await channelsApi(guildId).find(channelId);
                callback(guildId, channel);
            } catch (e) {
                console.log(e);
            }
        });
    }

    async close(): Promise<void> {
        const connection = await this.getConnection();
        await connection.stop();
        this._connection = undefined;
    }

    static get instance(): HubService {
        return hubService;
    }
}

const hubService = new HubService();

export default hubService;
