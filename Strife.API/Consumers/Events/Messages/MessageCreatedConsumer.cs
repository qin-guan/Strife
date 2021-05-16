using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Messages;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Messages
{
    public class MessageCreatedConsumer : IConsumer<IMessageCreated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public MessageCreatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IMessageCreated> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync(MessageMethods.Created, context.Message.GuildId, context.Message.ChannelId,
                    context.Message.MessageId);
        }
    }
}