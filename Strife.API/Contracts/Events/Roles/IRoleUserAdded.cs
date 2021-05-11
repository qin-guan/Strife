using System;

namespace Strife.API.Contracts.Events.Roles
{
    public interface IRoleUserAdded
    {
        public string RoleName { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}