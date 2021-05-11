using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guilds;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Guilds
{
    public class GuildCreatedConsumer : IConsumer<IGuildCreated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public GuildCreatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IGuildCreated> context)
        {
            await _hubContext.Clients.User(context.Message.InitiatedBy.ToString())
                .SendAsync(GuildMethods.Created, context.Message.GuildId);
        }
    }
}