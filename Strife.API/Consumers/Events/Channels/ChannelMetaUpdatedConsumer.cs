using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Channels;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Channels
{
    public class ChannelMetaUpdatedConsumer : IConsumer<IChannelMetaUpdated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public ChannelMetaUpdatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IChannelMetaUpdated> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync(ChannelMethods.MetaUpdated, context.Message.GuildId, context.Message.ChannelId);
        }
    }
}