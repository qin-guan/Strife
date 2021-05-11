using System;

namespace Strife.API.Contracts.Commands.Channels
{
    public interface ICreateChannel
    {
        public Guid GuildId { get; }
        public Guid ChannelId { get; }
        public string Name { get; }
        public bool IsVoice { get; }
        public string GroupName { get; }
        public Guid InitiatedBy { get; }
    }
}