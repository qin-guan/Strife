using System;

namespace Strife.API.Contracts.Commands.Guilds
{
    public interface ICreateGuild
    {
        public Guid GuildId { get; }
        public string Name { get; }
        public Guid InitiatedBy { get; }
    }
}