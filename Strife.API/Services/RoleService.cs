using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

using Strife.API.Interfaces;

using Strife.Configuration.Database;
using Strife.Configuration.Guild;

namespace Strife.API.Services
{
    public class RoleService : IRoleService
    {
        private readonly StrifeDbContext _context;
        private readonly RoleManager<GuildRole> _roleManager;
        public RoleService(StrifeDbContext context, RoleManager<GuildRole> roleManager)
        {
            _context = context;
            _roleManager = roleManager;
        }

        public async Task CreateGuildDefaultsAsync(Guid guildId)
        {
            await _roleManager.CreateAsync(new GuildRole
            {
                GuildId = guildId,
                Name = ""
            });
        }

    }
}