using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authorization
{
    public class PermissionRequirement : IAuthorizationRequirement
    {
        public string PermissionName { get; }

        public PermissionRequirement(string permissionName)
        {
            PermissionName = permissionName ?? throw new ArgumentNullException(nameof(permissionName));
        }
    }
}
