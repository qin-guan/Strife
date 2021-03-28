using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

using Strife.Configuration.Guild;
using Strife.Configuration.User;

namespace Strife.API.Policies
{
    public class HasPolicyHandler : AuthorizationHandler<HasPolicyRequirement, Guid>
    {
        private readonly RoleManager<GuildRole> _roleManager;
        private readonly UserManager<StrifeUser> _userManager;
        public HasPolicyHandler(RoleManager<GuildRole> roleManager, UserManager<StrifeUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, HasPolicyRequirement requirement, Guid resourceId)
        {
            var user = await _userManager.GetUserAsync(context.User);
            if (user is null) return;

            var roles = await Task.WhenAll((await _userManager.GetRolesAsync(user)).Select(_roleManager.FindByNameAsync));
            var claims = (await Task.WhenAll(roles.Select(_roleManager.GetClaimsAsync))).SelectMany(i => i).Select(claim => claim.Type);

            // For example, the policy name might be `Guild/CreateChannels`
            var split = requirement.PolicyName.Split("/");
            var requiredClaim = $"{split[0]}/{resourceId}/{split[1]}";

            if (claims.Contains(requiredClaim))
                context.Succeed(requirement);
        }
    }
}