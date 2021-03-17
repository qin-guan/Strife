using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Guild
{
    public class GuildCreatedConsumer : IConsumer<IGuildCreated>
    {
        private readonly IHubContext<GuildHub> _guildHub;

        public GuildCreatedConsumer(IHubContext<GuildHub> guildHub)
        {
            _guildHub = guildHub;
        }

        public async Task Consume(ConsumeContext<IGuildCreated> context)
        {
            await _guildHub.Clients.User(context.Message.InitiatedBy.ToString())
                .SendAsync("Guild/Created", context.Message.GuildId);
        }
    }
}