using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Serilog;
using SJInovacao.Acesso.Common.Security.Authentication.GroupAccess;
using SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Common.Security.Authentication
{
    public class HybridPolicyProvider : IAuthorizationPolicyProvider
    {
        private const string PERMISSION_PREFIX = "Permission";
        private const string GROUP_PREFIX = "Group";
        private readonly DefaultAuthorizationPolicyProvider _fallbackPolicyProvider;

        public HybridPolicyProvider(IOptions<AuthorizationOptions> options)
        {
            _fallbackPolicyProvider = new DefaultAuthorizationPolicyProvider(options);
        }

        public Task<AuthorizationPolicy> GetDefaultPolicyAsync()
            => _fallbackPolicyProvider.GetDefaultPolicyAsync();

        public Task<AuthorizationPolicy?> GetFallbackPolicyAsync()
            => _fallbackPolicyProvider.GetFallbackPolicyAsync();

        public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
        {
            Log.Information("🔐 [HybridPolicyProvider] Solicitando política: {PolicyName}", policyName);

            if (policyName.StartsWith($"{PERMISSION_PREFIX}:", StringComparison.OrdinalIgnoreCase))
            {
                var permissionsPart = policyName.Substring(PERMISSION_PREFIX.Length + 1);
                var permissions = permissionsPart
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(p => p.Trim())
                    .ToList();

                Log.Information("🔐 [HybridPolicyProvider] Criando política de Permissão com: {Permissions}", string.Join(", ", permissions));

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new PermissionRequirement(string.Join(",", permissions)));
                return Task.FromResult(policy.Build());
            }

            if (policyName.StartsWith($"{GROUP_PREFIX}:", StringComparison.OrdinalIgnoreCase))
            {
                var groupsPart = policyName.Substring(GROUP_PREFIX.Length + 1);
                var groups = groupsPart
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(g => g.Trim())
                    .ToList();

                Log.Information("🔐 [HybridPolicyProvider] Criando política de Grupo com: {Groups}", string.Join(", ", groups));

                var policy = new AuthorizationPolicyBuilder();
                policy.AddRequirements(new GroupRequirement(string.Join(",", groups)));
                return Task.FromResult(policy.Build());
            }

            Log.Warning("⚠️ [HybridPolicyProvider] Política não reconhecida, delegando para fallback: {PolicyName}", policyName);
            return _fallbackPolicyProvider.GetPolicyAsync(policyName);
        }
    }
}
