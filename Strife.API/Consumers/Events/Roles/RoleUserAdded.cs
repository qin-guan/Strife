using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Roles;
using Strife.API.Hubs;
using Strife.API.Hubs.Methods;

namespace Strife.API.Consumers.Events.Roles
{
    public class RoleUserAdded : IConsumer<IRoleUserAdded>
    {
        private readonly IHubContext<EventsHub> _hubContext;

        public RoleUserAdded(IHubContext<EventsHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task Consume(ConsumeContext<IRoleUserAdded> context)
        {
            await _hubContext.Clients.Group(context.Message.GuildId.ToString()).SendAsync(RoleMethods.UserAdded, new
            {
                context.Message.RoleName,
                context.Message.GuildId,
                UserId = context.Message.InitiatedBy,
            });
        }
    }
}