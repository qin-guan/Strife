using System;
using System.Collections.Generic;

namespace Strife.API.Contracts.Commands.Guild
{
    public interface ICreateGuildRole
    {
        public Guid GuildId { get; }
        public string Name { get; }
        public bool InternalRole { get; }
        public IEnumerable<string> Policies { get; }
        public int AccessLevel { get; }
        public Guid InitiatedBy { get; }
    }
}