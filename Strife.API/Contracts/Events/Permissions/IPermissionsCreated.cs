using System;

namespace Strife.API.Contracts.Events.Permissions
{
    public interface IPermissionsCreated
    {
        public Guid GuildId { get; }
        public string RoleName { get; }
        public Guid InitiatedBy { get; }
    }
}