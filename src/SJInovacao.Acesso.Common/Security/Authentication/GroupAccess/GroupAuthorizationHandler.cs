using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace SJInovacao.Acesso.Common.Security.Authentication.GroupAccess
{
    public class GroupAuthorizationHandler : AuthorizationHandler<GroupRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, GroupRequirement requirement)
        {
            var userGroups = context.User.Claims
                .Where(c => c.Type.EndsWith("groups", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            Log.Information("🔐 [GroupHandler] Claims recebidos: {Claims}", string.Join(", ", userGroups));
            Log.Information("🔐 [GroupHandler] Requisito: {Requirement}", string.Join(", ", requirement.Groups));

            var hasAnyGroup = requirement.Groups
                .Any(g => userGroups.Contains(g, StringComparer.OrdinalIgnoreCase));

            if (hasAnyGroup)
            {
                Log.Information("✅ [GroupHandler] Acesso concedido");
                context.Succeed(requirement);
            }
            else
            {
                Log.Warning("❌ [GroupHandler] Acesso negado");
                context.Fail();
            }

            return Task.CompletedTask;
        }


    }
}
