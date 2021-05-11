using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Serilog;
using Strife.API.Contracts.Commands.Roles;
using Strife.API.Contracts.Events.Roles;
using Strife.Core.Guilds;

namespace Strife.API.Consumers.Commands.Roles
{
    public class CreateRoleConsumer : IConsumer<ICreateRole>
    {
        private readonly RoleManager<GuildRole> _roleManager;

        public CreateRoleConsumer(RoleManager<GuildRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task Consume(ConsumeContext<ICreateRole> context)
        {
            await _roleManager.CreateAsync(new GuildRole
            {
                Name = $"Guilds/{context.Message.GuildId}/Roles/{context.Message.Name}",
                AccessLevel = context.Message.AccessLevel,
                GuildId = context.Message.GuildId,
                InternalRole = context.Message.InternalRole
            });

            await context.Publish<IRoleCreated>(new
            {
                context.Message.GuildId, RoleName = context.Message.Name, context.Message.InitiatedBy
            });
        }
    }
}