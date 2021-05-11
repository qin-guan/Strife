using System;
using System.Collections.Generic;

namespace Strife.API.Contracts.Commands.Permissions
{
    public interface ICreatePermissions
    {
        public Guid GuildId { get; }
        public string RoleName { get; }
        public IEnumerable<string> PermissionStrings { get; }
        public Guid InitiatedBy { get; }
    }
}