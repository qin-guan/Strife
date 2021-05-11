using System;

namespace Strife.API.Contracts.Events.Guilds
{
    public interface IGuildUserAdded
    {
        public Guid GuildId { get; }
        
        public Guid InitiatedBy { get; }
    }
}