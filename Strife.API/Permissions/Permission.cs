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
        public bool IsWild { get; set; }
        public ResourceType ResourceType { get; set; }
        public PermissionAllowDeny AllowDeny { get; set; }
        public PermissionOperationType OperationType { get; set; }

        // To be used to verify permissions only
        public Permission(Guid guildId, Guid resourceId, ResourceType resourceType,
            PermissionOperationType operationType)
        {
            GuildId = guildId;
            ResourceId = resourceId;
            ResourceType = resourceType;
            OperationType = operationType;
        }

        // To be used to create wild permissions
        public Permission(Guid guildId, ResourceType resourceType, PermissionAllowDeny allowDeny,
            PermissionOperationType operationType)
        {
            GuildId = guildId;
            IsWild = true;
            ResourceType = resourceType;
            AllowDeny = allowDeny;
            OperationType = operationType;
        }

        public Permission(string policy)
        {
            var parsed = policy.Split("/");
            if (parsed.Length != 6) throw new Exception("Invalid permission string");

            if (!Guid.TryParse(parsed[1], out var guildId)) throw new Exception("Invalid Guild Id");
            GuildId = guildId;

            if (!Enum.TryParse(parsed[2], out ResourceType resourceType))
                throw new Exception("Invalid resource type");
            ResourceType = resourceType;

            if (Guid.TryParse(parsed[3], out var resourceId))
            {
                ResourceId = resourceId;
            }
            else if (parsed[3] == "*")
            {
                IsWild = true;
            }
            else
            {
                throw new Exception("Invalid Resource Id");
            }

            if (!Enum.TryParse(parsed[4], out PermissionOperationType operationType))
                throw new Exception("Invalid operation type");
            OperationType = operationType;

            if (!Enum.TryParse(parsed[5], out PermissionAllowDeny allowDeny)) throw new Exception("Invalid allow deny");
            AllowDeny = allowDeny;
        }

        public static IEnumerable<Permission> AllowAll(Guid guildId)
        {
            var permissions = new List<Permission>();
            foreach (var resourceType in Enum.GetValues<ResourceType>())
            {
                permissions.AddRange(
                    Enum.GetValues<PermissionOperationType>().Select(op =>
                        new Permission(guildId, resourceType,
                            PermissionAllowDeny.Allow, op)));
            }

            return permissions;
        }

        /// <summary>
        /// This should generate strings representing the Permission. Permissions can be wildcards.
        ///
        /// For example:
        /// A permission to allow creating roles in a guild would be a wildcard, as there is no specific role resource specified.
        /// The resulting permission string would be $"Guilds/{guildId}/Roles/*/Create/Allow"
        ///
        /// A permission to allow updating role details would not be a wildcard, as the resource is the role itself.
        /// The resulting permission string would be $"Guild/{guildId}/Role/{roleId}/Update/Allow"
        ///
        /// Tbh, I don't know if this makes sense either.
        /// </summary>
        /// <returns>string</returns>
        public override string ToString() =>
            $"Guild/{GuildId}/{ResourceType}/{(IsWild ? "*" : ResourceId)}/{OperationType}/{AllowDeny}";
    }
}