using System;

namespace Strife.API.Contracts.Events.Messages
{
    public interface IMessageCreated
    {
        public Guid MessageId { get; }
        public Guid GuildId { get; }
        public Guid ChannelId { get; }
        public Guid InitiatedBy { get; }
    }
}