using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Hubs;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Hubs
{
    public class GuildSubscribedByUserConsumer: IConsumer<IGuildSubscribedByUser>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public GuildSubscribedByUserConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }
        
        public async Task Consume(ConsumeContext<IGuildSubscribedByUser> context)
        {
            await _hubContext.Groups.AddToGroupAsync(context.Message.ConnectionId, context.Message.GuildId.ToString());
        }
    }
}