using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Guild
{
    public class GuildUserAddedConsumer : IConsumer<IGuildUserAdded>
    {
        private readonly IHubContext<GuildHub> _guildHub;

        public GuildUserAddedConsumer(IHubContext<GuildHub> guildHub)
        {
            _guildHub = guildHub;
        }

        public async Task Consume(ConsumeContext<IGuildUserAdded> context)
        {
            await _guildHub.Clients.Group(context.Message.GuildId.ToString()).SendAsync("Guild/UserAdded", context.Message.InitiatedBy);
        }
    }
}