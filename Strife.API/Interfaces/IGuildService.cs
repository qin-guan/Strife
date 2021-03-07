using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Strife.Configuration.Guild;

namespace Strife.API.Interfaces
{
    public interface IGuildService
    {
        Task<Guild> FindAsync(Guid guildId);
        Task<IEnumerable<Guild>> FindByUserIdAsync(Guid userId);
        Task<Guild> AddAsync(Guild guild);
    }
}