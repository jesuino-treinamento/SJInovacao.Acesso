using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Authorization.GroupAccess
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
