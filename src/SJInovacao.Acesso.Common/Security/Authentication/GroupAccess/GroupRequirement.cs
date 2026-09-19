using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication.GroupAccess
{
    public class GroupRequirement : IAuthorizationRequirement
    {
        public IReadOnlyList<string> Groups { get; }

        public GroupRequirement(string groups)
        {
            Groups = groups
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();
        }
    }
}
