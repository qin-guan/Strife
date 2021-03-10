import { hostnames } from "../api/http/Base";
import * as signalR from "@microsoft/signalr";

import authorizationService from "../oidc/AuthorizationService";

import { GuildInstance } from "../models/guild/Guild";
import { find } from "../api/http/Guilds";

export const GuildContracts = {
    Commands: {},
    Events: {
        GuildCreated: "GUILD.CREATED",
    },
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

      connection.on(GuildContracts.Events.GuildCreated, async (id: string) => {
          const guild = await find(id);
          callback(guild);
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
