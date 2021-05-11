using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MassTransit;
using Strife.API.Contracts.Events.Permissions;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Permissions
{
    public class PermissionsCreatedConsumer : IConsumer<IPermissionsCreated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public PermissionsCreatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IPermissionsCreated> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync("Roles/Updated", context.Message.RoleName);
        }
    }
}