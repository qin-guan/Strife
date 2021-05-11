using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guilds;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Guilds
{
    public class GuildUserAddedConsumer : IConsumer<IGuildUserAdded>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public GuildUserAddedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IGuildUserAdded> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync(GuildMethods.UserAdded, context.Message.InitiatedBy);
        }
    }
}