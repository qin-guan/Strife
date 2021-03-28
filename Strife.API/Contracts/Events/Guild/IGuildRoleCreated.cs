using System;

namespace Strife.API.Contracts.Events.Guild
{
    public interface IGuildRoleCreated
    {
        public string RoleName { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}