using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess
{
    public class PermissionAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX_PERMISSION= "Permission";

        public PermissionAttribute(params string[] permissions)
        {
            Policy = $"{POLICY_PREFIX_PERMISSION}:{string.Join(",", permissions)}";
        }
    }
}


