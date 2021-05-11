using System;

namespace Strife.API.Contracts.Events.Roles
{
    public interface IRoleCreated
    {
        public string RoleName { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}