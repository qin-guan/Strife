using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Contracts.Commands.Roles;
using Strife.API.Contracts.Events.Roles;
using Strife.Core.Database;
using Strife.Core.Guilds;

namespace Strife.API.Consumers.Commands.Roles
{
    public class AddRoleUserConsumer : IConsumer<IAddRoleUser>
    {
        private readonly StrifeDbContext _dbContext;

        public AddRoleUserConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<IAddRoleUser> context)
        {
            var role = await _dbContext.Roles.SingleOrDefaultAsync(r =>
                r.Name == $"Guilds/{context.Message.GuildId}/Roles/{context.Message.RoleName}");
            if (role == default(GuildRole)) throw new Exception("Role was not found");
            
            await _dbContext.UserRoles.AddAsync(new IdentityUserRole<Guid>
            {
                RoleId = role.Id,
                UserId = context.Message.InitiatedBy
            });
            await _dbContext.SaveChangesAsync();
            
            await context.Publish<IRoleUserAdded>(new
            {
                context.Message.RoleName,
                context.Message.GuildId,
                context.Message.InitiatedBy
            });
        }
    }
}