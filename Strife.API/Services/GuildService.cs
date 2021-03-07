using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Strife.Configuration.Guild;
using Strife.Configuration.Database;
using Strife.API.Interfaces;

namespace Strife.API.Services
{
    public class GuildService : IGuildService
    {
        private readonly StrifeDbContext _context;

        public GuildService(StrifeDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Guild>> FindByUserIdAsync(Guid userId)
        {
            return await _context.GuildStrifeUser
                .Include(gsu => gsu.Guild)
                .Where(gsu => gsu.UserId == userId)
                .OrderBy(gsu => gsu.Sequence)
                .Select(gsu => gsu.Guild)
                .ToListAsync();
        }

        public async Task<Guild> AddAsync(Guild guild)
        {
            var newGuild = await _context.Guilds.AddAsync(guild);
            await _context.SaveChangesAsync();
            return newGuild.Entity;
        }
    }
}