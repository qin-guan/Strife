import { hostnames } from "../api/http/Base";
import * as signalR from "@microsoft/signalr";

import authorizationService from "../oidc/AuthorizationService";

import { IGuild } from "../models/guild/Guild";
import { guilds } from "../api/http/Guilds";

export const GuildContracts = {
    Commands: {},
    Events: {
        GuildCreated: "GUILD.CREATED",
    },
};

export class GuildHubService {
  _connection?: signalR.HubConnection;

  async ensureConnectionExists() {
      if (this._connection !== undefined) return;
      this._connection = new signalR.HubConnectionBuilder()
          .withUrl(`${hostnames.api}/hubs/guild`, {
              accessTokenFactory: async () =>
                  (await authorizationService.getAccessToken()) ?? "",
          })
          .build();
      try {
          await this._connection.start();
      } catch (error) {
          console.error(error);
          throw error;
      }
  }

  async getConnection(): Promise<{ connection: signalR.HubConnection }> {
      await this.ensureConnectionExists();
      return { connection: this._connection! };
  }

  async onGuildCreated(callback: (guild: IGuild) => void) {
      const { connection } = await this.getConnection();

      connection.on(GuildContracts.Events.GuildCreated, async (id: string) => {
          const guild = await guilds.find({ id });
          callback(guild);
      });
  }

  static get instance() {
      return guildHubService;
  }
}

const guildHubService = new GuildHubService();

export default guildHubService;
