using System;
using System.Collections.Generic;

namespace Strife.API.Contracts.Commands.Guild
{
    public interface ICreateGuildRole
    {
        public Guid GuildId { get; }
        public string Name { get; }
        public IEnumerable<string> Policies { get; }
        public Guid InitiatedBy { get; }
    }
}