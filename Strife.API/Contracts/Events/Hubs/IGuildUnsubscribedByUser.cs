using System;

namespace Strife.API.Contracts.Events.Hubs
{
    public interface IGuildUnsubscribedByUser
    {
        public string ConnectionId { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}