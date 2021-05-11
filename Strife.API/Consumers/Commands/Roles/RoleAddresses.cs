using System;

namespace Strife.API.Consumers.Commands.Roles
{
    public class RoleAddresses
    {
        public static readonly Uri AddRoleUserConsumer = new("queue:AddRoleUser");
        public static readonly Uri CreateRoleConsumer = new("queue:CreateRole");
    }
}