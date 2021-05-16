using System;
using Microsoft.AspNetCore.Authorization;
using Strife.Core.Resources;

namespace Strife.API.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; set; }

        public PermissionRequirement(string permission)
        {
            Permission = new Permission(permission);
        }

        public string ToWildString() => Permission.ChildResource is null
            ? $"Guild/{Permission.GuildId}/{Permission.ResourceType}/*/{Permission.OperationType}"
            : $"Guild/{Permission.GuildId}/{Permission.ResourceType}/*/{Permission.ChildResource.ResourceType}/*/{Permission.OperationType}";

        public override string ToString() => Permission.ChildResource is null
            ? $"Guild/{Permission.GuildId}/{Permission.ResourceType}/{Permission.ResourceId}/{Permission.OperationType}"
            : $"Guild/{Permission.GuildId}/{Permission.ResourceType}/{Permission.ResourceId}/{Permission.ChildResource.ResourceType}/{Permission.ResourceId}/{Permission.OperationType}";
    }
}