using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX_PERMISSION = "Permission";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith($"{POLICY_PREFIX_PERMISSION}:", StringComparison.OrdinalIgnoreCase))
            {
                var permissionName = policyName.Substring(POLICY_PREFIX_PERMISSION.Length + 1);
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permissionName));
                return Task.FromResult(policy.Build());
            }
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
