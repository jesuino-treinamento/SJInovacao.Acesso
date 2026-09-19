using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication.GroupAccess
{
    public class GroupAttribute : AuthorizeAttribute
    {
        private const string POLICY_PREFIX_GROUP = "Group";

        public GroupAttribute(params string[] groups)
        {
            Policy = $"{POLICY_PREFIX_GROUP}:{string.Join(",", groups)}";
        }
    }
}


