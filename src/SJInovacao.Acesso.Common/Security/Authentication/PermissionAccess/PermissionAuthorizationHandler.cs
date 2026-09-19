using Microsoft.AspNetCore.Authorization;
using Serilog;

namespace SJInovacao.Acesso.Common.Security.Authentication.PermissionAccess
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var userPermissions = context.User.Claims
                .Where(c => c.Type.EndsWith("permissions", StringComparison.OrdinalIgnoreCase))
                .Select(c => c.Value)
                .ToList();

            Log.Information("🔐 [PermissionHandler] Claims recebidos: {Claims}", string.Join(", ", userPermissions));
            Log.Information("🔐 [PermissionHandler] Requisito: {Requirement}", string.Join(", ", requirement.Permissions));

            var hasAnyPermission = requirement.Permissions
                .Any(p => userPermissions.Contains(p, StringComparer.OrdinalIgnoreCase));

            if (hasAnyPermission)
            {
                Log.Information("✅ [PermissionHandler] Acesso concedido");
                context.Succeed(requirement);
            }
            else
            {
                Log.Warning("❌ [PermissionHandler] Acesso negado");
                context.Fail();
            }

            return Task.CompletedTask;
        }


    }
}
