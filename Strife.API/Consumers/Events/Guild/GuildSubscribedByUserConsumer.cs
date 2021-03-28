using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Guild
{
    public class GuildSubscribedByUserConsumer: IConsumer<IGuildSubscribedByUser>
    {
        private readonly IHubContext<GuildHub> _hubContext;

        public GuildSubscribedByUserConsumer(IHubContext<GuildHub> hubContext)
        {
            _hubContext = hubContext;
        }
        public async Task Consume(ConsumeContext<IGuildSubscribedByUser> context)
        {
            await _hubContext.Groups.AddToGroupAsync(context.Message.ConnectionId, context.Message.GuildId.ToString());
        }
    }
}