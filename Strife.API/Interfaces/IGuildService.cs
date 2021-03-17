using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Strife.Configuration.Guild;
using Strife.Configuration.Joins;

namespace Strife.API.Interfaces
{
    public interface IGuildService
    {
        Task<Guild> FindByIdAsync(Guid guildId);
        Task<IEnumerable<Guild>> FindByUserIdAsync(Guid userId);
        Task<Guild> AddAsync(Guild guild);
        Task<GuildStrifeUser> AddUserAsync(Guid guildId, Guid userId);
    }
}