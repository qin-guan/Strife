using System;

namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildCreated
    {
        public Guid GuildId { get; }
    }
}