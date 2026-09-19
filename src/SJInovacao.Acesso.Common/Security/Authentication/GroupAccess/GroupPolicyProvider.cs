using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SJInovacao.Acesso.Common.Security.Authentication.GroupAccess
{
    public class GroupPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX_GROUP = "Group";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public GroupPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith($"{POLICY_PREFIX_GROUP}:", StringComparison.OrdinalIgnoreCase))
            {
                var groupName = policyName.Substring(POLICY_PREFIX_GROUP.Length + 1);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new GroupRequirement(groupName));
                return Task.FromResult(policy.Build());
            }
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
