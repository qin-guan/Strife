using System;

namespace Strife.API.Contracts.Events.Channels
{
    public interface IChannelCreated
    {
        public Guid ChannelId { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}