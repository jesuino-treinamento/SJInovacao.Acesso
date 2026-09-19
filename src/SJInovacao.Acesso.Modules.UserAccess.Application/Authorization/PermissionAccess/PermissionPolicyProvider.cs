using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authorization.PermissionAccess
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
                var permissionsPart = policyName.Substring(POLICY_PREFIX_PERMISSION.Length + 1);

                // 🔹 Suporte a múltiplas permissões separadas por vírgula
                var permissions = permissionsPart
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(string.Join(",", permissions)));
                return Task.FromResult(policy.Build());
            }

            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
