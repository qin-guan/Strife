using System;

namespace Strife.API.Contracts.Events.Guilds
{
    public interface IGuildCreated
    {
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}