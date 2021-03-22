using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Strife.API.Policies
{
    public class HasPolicyRequirement : IAuthorizationRequirement
    {
        public string PolicyName { get; }
        public HasPolicyRequirement(string policyName)
        {
            PolicyName = policyName ?? throw new ArgumentNullException(nameof(policyName));
        }
    }
}