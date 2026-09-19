using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public IReadOnlyList<string> Permissions { get; }

        public PermissionRequirement(string permissions)
        {
            Permissions = permissions
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        }
    }
}
