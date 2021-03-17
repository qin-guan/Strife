using System;

namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildUserAdded
    {
        public Guid GuildId { get; }
        
        public Guid InitiatedBy { get; }
    }
}