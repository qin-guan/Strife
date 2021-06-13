using System;

namespace Strife.API.Contracts.Events.Channels
{
    public interface IChannelMetaUpdated
    {
        public Guid ChannelId { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}