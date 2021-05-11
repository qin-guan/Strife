using System;

namespace Strife.API.Contracts.Commands.Guilds
{
    public interface IAddGuildUser
    {
        public Guid GuildId { get; }
        
        public Guid InitiatedBy { get; }
    }
}