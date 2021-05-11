using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Strife.API.Permissions;

namespace Strife.API.Providers
{
    public class AuthorizationPolicyProvider : DefaultAuthorizationPolicyProvider
    {
        private readonly AuthorizationOptions _options;

        public AuthorizationPolicyProvider(IOptions<AuthorizationOptions> options) : base(options)
        {
            _options = options.Value;
        }

        public override async Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
        {
            var policy = await base.GetPolicyAsync(policyName);

            if (policy != null) return policy;
            
            policy = new AuthorizationPolicyBuilder()
                .AddRequirements(new PermissionRequirement(policyName))
                .Build();

            _options.AddPolicy(policyName, policy);

            return policy;
        }
    }
}