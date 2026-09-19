using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authorization.GroupAccess
{
    public class GroupPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string POLICY_PREFIX_GROUP = "Group";
        private readonly DefaultAuthorizationPolicyProvider _fallbackProvider;

        public GroupPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            if (policyName.StartsWith($"{POLICY_PREFIX_GROUP}:", StringComparison.OrdinalIgnoreCase))
            {
                var groupsPart = policyName.Substring(POLICY_PREFIX_GROUP.Length + 1);

                // 🔹 Suporte a múltiplas permissões separadas por vírgula
                var groups = groupsPart
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new GroupRequirement(string.Join(",", groups)));
                return Task.FromResult(policy.Build());
            }

            return _fallbackProvider.GetPolicyAsync(policyName);
        }
    }
}
