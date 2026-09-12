using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX = "Permission";

        public PermissionAttribute(string permissionName)
        {
            Policy = $"{POLICY_PREFIX}:{permissionName}";
        }
    }
}


