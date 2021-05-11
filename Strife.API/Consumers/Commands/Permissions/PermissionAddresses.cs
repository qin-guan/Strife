using System;

namespace Strife.API.Consumers.Commands.Permissions
{
    public class PermissionAddresses
    { 
        public static Uri CreatePermissionsConsumer = new("queue:CreatePermissions");
    }
}