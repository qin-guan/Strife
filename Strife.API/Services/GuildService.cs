using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

using Strife.Configuration.Guild;
using Strife.Configuration.Database;
using Strife.API.Interfaces;
using Strife.Configuration.Joins;

namespace Strife.API.Services
{
    public class GuildService : IGuildService
    {
        private readonly StrifeDbContext _context;

        public GuildService(StrifeDbContext context)
        {
            _context = context;
        }

        public async Task<Guild> FindByIdAsync(Guid guildId)
        {
            return await _context.Guilds.FindAsync(guildId);
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
        
        public async Task<GuildStrifeUser> AddUserAsync(Guid guildId, Guid userId)
        {
            var guilds = _context.GuildStrifeUser.Where(gsu => gsu.UserId == userId).ToList();
            var sequence = !guilds.Any() ? 0 : guilds.Max(g => g.Sequence);

            var join = new GuildStrifeUser 
            {
                UserId = userId,
                GuildId = guildId,
                Sequence = sequence + 1
            };

            var newJoin = await _context.GuildStrifeUser.AddAsync(join);
            await _context.SaveChangesAsync();

            return newJoin.Entity;
        }

    }
}