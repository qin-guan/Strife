using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.OpenApi.Models;
using Strife.Core.Resources;

namespace Strife.API.Permissions
{
    public class Permission
    {
        public const string ClaimType = "Permission";

        public Guid GuildId { get; set; }

        public Guid ResourceId { get; set; }
        public bool IsWild => ResourceId == Guid.Empty;
        public ResourceType ResourceType { get; set; }
        public ChildResource ChildResource { get; set; }
        public PermissionAllowDeny AllowDeny { get; set; }
        public PermissionOperationType OperationType { get; set; }

        public Permission()
        {
        }

        // To check authorization 
        public Permission(Guid guildId, Guid resourceId, ResourceType resourceType,
            PermissionOperationType operationType, PermissionAllowDeny allowDeny)
        {
            GuildId = guildId;
            ResourceId = resourceId;
            ResourceType = resourceType;
            OperationType = operationType;
            AllowDeny = allowDeny;
        }

        public Permission(Guid guildId, Guid resourceId, ResourceType resourceType,
            PermissionOperationType operationType, PermissionAllowDeny allowDeny, ChildResource childResource)
        {
            GuildId = guildId;
            ResourceId = resourceId;
            ResourceType = resourceType;
            OperationType = operationType;
            AllowDeny = allowDeny;
            ChildResource = childResource;
        }

        // To create wild permissions
        public Permission(Guid guildId, ResourceType resourceType, PermissionOperationType operationType,
            PermissionAllowDeny allowDeny)
        {
            GuildId = guildId;
            ResourceType = resourceType;
            AllowDeny = allowDeny;
            OperationType = operationType;
        }

        public Permission(Guid guildId, ResourceType resourceType, PermissionOperationType operationType,
            PermissionAllowDeny allowDeny, ChildResource childResource)
        {
            GuildId = guildId;
            ResourceType = resourceType;
            AllowDeny = allowDeny;
            OperationType = operationType;
            ChildResource = childResource;
        }

        public Permission(string permission)
        {
            var parsed = permission.Split("/");

            if (parsed.Length > 8 ||
                parsed[0] != "Guild" ||
                !Enum.TryParse(parsed[^1], out PermissionAllowDeny allowDeny) ||
                !Enum.TryParse(parsed[^2], out PermissionOperationType operationType) ||
                !Guid.TryParse(parsed[1], out var guildId) ||
                !Enum.TryParse(parsed[2], out ResourceType resourceType))
                throw new Exception("Invalid permission string");

            if (parsed.Length > 6)
            {
                if (!Enum.TryParse(parsed[4], out ResourceType childResourceType))
                    throw new Exception("Invalid permission string");

                ChildResource = new ChildResource
                {
                    ResourceType = childResourceType
                };

                if (Guid.TryParse(parsed[5], out var childResourceId))
                    ChildResource.ResourceId = childResourceId;
            }

            if (Guid.TryParse(parsed[3], out var resourceId)) ResourceId = resourceId;

            ResourceType = resourceType;
            AllowDeny = allowDeny;
            OperationType = operationType;
            GuildId = guildId;
        }

        public static IEnumerable<Permission> AllowAll(Guid guildId)
        {
            var permissions = new List<Permission>();
            var ops = Enum.GetValues<PermissionOperationType>();
            foreach (var resourceType in Enum.GetValues<ResourceType>())
            {
                if (resourceType == ResourceType.Message) continue;
                permissions.AddRange(
                    ops.Select(op =>
                        new Permission(guildId, resourceType,
                            op, PermissionAllowDeny.Allow)));
            }

            permissions.AddRange(ops.Select(op =>
                new Permission(guildId, ResourceType.Channel, op, PermissionAllowDeny.Allow, new ChildResource
                {
                    ResourceType = ResourceType.Message
                })));

            return permissions;
        }

        /// <summary>
        /// This should generate strings representing the Permission. Permissions can be wildcards.
        ///
        /// For example:
        /// A permission to allow creating roles in a guild would be a wildcard, as there is no specific role resource specified.
        /// The resulting permission string would be $"Guilds/{guildId}/Roles/*/Create/Allow"
        /// $"Guilds/{guildId}/Channels/{channelId}/Messages/Create/Allow"
        /// 
        /// A permission to allow updating role details would not be a wildcard, as the resource is the role itself.
        /// The resulting permission string would be $"Guild/{guildId}/Role/{roleId}/Update/Allow"
        ///
        /// Tbh, I don't know if this makes sense either.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString() => ChildResource is null
            ? $"Guild/{GuildId}/{ResourceType}/{(IsWild ? "*" : ResourceId)}/{OperationType}/{AllowDeny}"
            : $"Guild/{GuildId}/{ResourceType}/{(IsWild ? "*" : ResourceId)}/{ChildResource.ResourceType}/{(ChildResource.IsWild ? "*" : ChildResource.ResourceId)}/{OperationType}/{AllowDeny}";
    }
}