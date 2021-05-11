using System;
using System.Linq;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Strife.API.Contracts.Commands.Permissions;
using Strife.API.Contracts.Events.Permissions;
using Strife.API.Permissions;
using Strife.Core.Database;
using Strife.Core.Guilds;

namespace Strife.API.Consumers.Commands.Permissions
{
    public class CreatePermissionsConsumer : IConsumer<ICreatePermissions>
    {
        private readonly StrifeDbContext _dbContext;

        public CreatePermissionsConsumer(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Consume(ConsumeContext<ICreatePermissions> context)
        {
            Log.Error("called create permissions");
            var role = await _dbContext.Roles.SingleOrDefaultAsync(r =>
                r.Name == $"Guilds/{context.Message.GuildId}/Roles/{context.Message.RoleName}");
            if (role == default(GuildRole)) throw new Exception("Role was not found");

            await _dbContext.RoleClaims.AddRangeAsync(context.Message.PermissionStrings.Select(permission =>
                new IdentityRoleClaim<Guid>
                {
                    RoleId = role.Id,
                    ClaimType = Permission.ClaimType,
                    ClaimValue = permission
                }));
            await _dbContext.SaveChangesAsync();

            await context.Publish<IPermissionsCreated>(new
            {
                context.Message.GuildId,
                context.Message.RoleName,
                context.Message.InitiatedBy
            });
        }
    }
}