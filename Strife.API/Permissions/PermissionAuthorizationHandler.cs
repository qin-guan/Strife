using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Strife.Core.Database;
using Strife.Core.Resources;

namespace Strife.API.Permissions
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement, IResource>
    {
        private readonly StrifeDbContext _dbContext;

        public PermissionAuthorizationHandler(StrifeDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
            PermissionRequirement requirement, IResource resource)
        {
            if (!Guid.TryParse(context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value, out var userId))
                throw new UnauthorizedAccessException();

            // First we get all the user role joins for the user
            var userRoles = await _dbContext.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId)
                .ToListAsync();
            // Then we get the role and filter to only include for the project
            var roles = await _dbContext.Roles
                .Where(r => userRoles.Contains(r.Id) && r.GuildId == resource.Id).Select(r => r.Id).ToListAsync();

            // Explicit permission string
            var explicitString = requirement.ToString();
            // Wildcard permission string
            var wildString = requirement.ToWildString();

            // Get all the role claims 
            var roleClaims = await _dbContext.RoleClaims
                .Where(r => r.ClaimType == Permission.ClaimType && roles.Contains(r.RoleId)).ToListAsync();

            // Check if any of the claims contain the explicit string
            var explicitRoleClaim = roleClaims.FirstOrDefault(rc => rc.ClaimValue.Contains(explicitString));
            // If no explicit string exist
            if (explicitRoleClaim == default(IdentityRoleClaim<Guid>))
            {
                // Check if any of the claims contain the wildcard
                var wildStringRoleClaim = roleClaims.FirstOrDefault(rc => rc.ClaimValue.Contains(wildString));
                // If there isn't a wildcard, we fail the requirement
                if (wildStringRoleClaim == default(IdentityRoleClaim<Guid>)) return;

                if (new Permission(wildStringRoleClaim.ClaimValue).AllowDeny == PermissionAllowDeny.Allow)
                    context.Succeed(requirement);
            }
            // Explicit string exists
            else
            {
                if (new Permission(explicitRoleClaim.ClaimValue).AllowDeny == PermissionAllowDeny.Allow)
                    context.Succeed(requirement);
            }
        }
    }
}