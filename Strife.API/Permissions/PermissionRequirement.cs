using System;
using Microsoft.AspNetCore.Authorization;
using Strife.Core.Resources;

namespace Strife.API.Permissions
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public Permission Permission { get; set; }
        
        public Guid GuildId { get; set; }
        public Guid ResourceId { get; set; }
        public ResourceType ResourceType { get; set; }
        public PermissionOperationType OperationType { get; set; }
        public PermissionRequirement(string permission)
        {
            Permission = new Permission(permission);
            GuildId = Permission.GuildId;
            ResourceId = Permission.ResourceId;
            ResourceType = Permission.ResourceType;
            OperationType = Permission.OperationType;
        }

        public string ToWildString() =>
            $"Guild/{GuildId}/{ResourceType}/*/{OperationType}";
        
        public override string ToString() =>
            $"Guild/{GuildId}/{ResourceType}/{ResourceId}/{OperationType}";
    }
}