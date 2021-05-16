using System;

namespace Strife.API.Contracts.Commands.Messages
{
    public interface ICreateMessage
    {
        public string Content { get; }
        public Guid GuildId { get; }
        public Guid ChannelId { get; }
        
        public Guid InitiatedBy { get; }
    }
}