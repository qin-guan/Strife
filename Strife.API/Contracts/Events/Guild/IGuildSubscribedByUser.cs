using System;

namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildSubscribedByUser
    {
        public string ConnectionId { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}