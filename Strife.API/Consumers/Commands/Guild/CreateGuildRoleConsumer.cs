using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MassTransit;

using Strife.API.Contracts.Commands.Guild;
using Strife.API.Contracts.Events.Guild;
using Strife.Configuration.Guild;

namespace Strife.API.Consumers.Commands.Guild
{
    public class CreateGuildRoleConsumer : IConsumer<ICreateGuildRole>
    {
        private readonly RoleManager<GuildRole> _roleManager;

        public CreateGuildRoleConsumer(RoleManager<GuildRole> roleManager)
        {
            _roleManager = roleManager;
        }
        
        public async Task Consume(ConsumeContext<ICreateGuildRole> context)
        {
            var roleName = $"Guild/{context.Message.GuildId.ToString()}/Role/{context.Message.Name}"; 
            await _roleManager.CreateAsync(new GuildRole
            {
                Name = roleName,
                GuildId = context.Message.GuildId,
                AccessLevel = context.Message.AccessLevel,
                InternalRole = context.Message.InternalRole
            });

            await context.Publish<IGuildRoleCreated>(new
            {
                RoleName = roleName,
                InitiatedBy = context.Message.InitiatedBy
            });
        }
    }
}