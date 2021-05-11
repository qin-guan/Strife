using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Serilog;
using Strife.API.Contracts.Events.Roles;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Roles
{
    public class RoleCreatedConsumer: IConsumer<IRoleCreated>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public RoleCreatedConsumer(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IRoleCreated> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync(RoleMethods.Created, context.Message.RoleName);
        }
    }
}