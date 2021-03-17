using System;

namespace Strife.API.Contracts.Commands.Guild
{
    public interface IAddGuildUser
    {
        public Guid GuildId { get; }
        
        public Guid InitiatedBy { get; }
    }
}