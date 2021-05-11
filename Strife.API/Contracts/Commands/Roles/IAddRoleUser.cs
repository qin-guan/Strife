using System;

namespace Strife.API.Contracts.Commands.Roles
{
    public interface IAddRoleUser
    {
        public string RoleName { get; }
        public Guid GuildId { get; }
        public Guid InitiatedBy { get; }
    }
}