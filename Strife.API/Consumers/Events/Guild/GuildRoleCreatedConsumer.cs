using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using Strife.API.Contracts.Events.Guild;
using Strife.API.Hubs;

namespace Strife.API.Consumers.Events.Guild
{
    public class GuildRoleCreatedConsumer: IConsumer<IGuildRoleCreated>
    {
        private readonly IHubContext<GuildHub> _guildHub;

        public GuildRoleCreatedConsumer(IHubContext<GuildHub> guildHub)
        {
            _guildHub = guildHub;
        }

        public async Task Consume(ConsumeContext<IGuildRoleCreated> context)
        {
            await _guildHub.Clients.Group(context.Message.GuildId.ToString())
                .SendAsync("Guild/RoleCreated", context.Message.RoleName);
        }
    }
}