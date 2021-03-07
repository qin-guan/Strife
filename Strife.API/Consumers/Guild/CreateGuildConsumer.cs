using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;

using Strife.API.Contracts.Events.Guild;
using Strife.API.Hubs;

using Strife.Configuration.Database;

namespace Strife.API.Consumers.Guild
{
    public class CreateGuildConsumer: IConsumer<IGuildCreated>
    {
        private readonly IHubContext<GuildHub> _guildHub;
        public CreateGuildConsumer(IHubContext<GuildHub> guildHub)
        {
            _guildHub = guildHub;
        }
        public Task Consume(ConsumeContext<IGuildCreated> context)
        {
            this._guildHub.Clients.Users(context.Message.UserIds).SendAsync("GUILD.CREATED", context.Message.GuildId);
            
            return Task.CompletedTask;
        }
    }
}