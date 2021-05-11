using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Channels;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Channels
{
    public class ChannelCreatedConsumer : IConsumer<IChannelCreated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public ChannelCreatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IChannelCreated> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync(ChannelMethods.Created, context.Message.GuildId, context.Message.ChannelId);
        }
    }
}