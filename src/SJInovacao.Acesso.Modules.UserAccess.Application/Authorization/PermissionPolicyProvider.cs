using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authorization
{
    public class PermissionPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX = "Permission";
        private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

        public PermissionPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackProvider.GetFallbackPolicyAsync()!;

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase))
            {
                var permission = policyName[(POLICY_PREFIX.Length + 1)..];
                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(permission));
                return Task.FromResult(policy.Build())!;
            }

            return _fallbackProvider.GetPolicyAsync(policyName)!;
        }
    }
}
