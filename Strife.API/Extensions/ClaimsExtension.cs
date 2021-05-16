using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Strife.API.Permissions;
using Strife.Core.Database;

namespace Strife.API.Extensions
{
    public static class ClaimsExtension
    {
        public static async Task<List<IdentityRoleClaim<Guid>>> GetClaimsAsync(this StrifeDbContext dbContext,
            Guid guildId, Guid userId)
        {
            // First we get all the user role joins for the user
            var userRoles = await dbContext.UserRoles.Where(ur => ur.UserId == userId).Select(ur => ur.RoleId)
                .ToListAsync();
            // Then we get the role and filter to only include for the project
            var roles = await dbContext.Roles
                .Where(r => userRoles.Contains(r.Id) && r.GuildId == guildId).Select(r => r.Id).ToListAsync();

            // Get all the role claims
            return await dbContext.RoleClaims
                .Where(r => r.ClaimType == Permission.ClaimType && roles.Contains(r.RoleId)).ToListAsync();
        }
    }
}