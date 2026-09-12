using Microsoft.AspNetCore.Authorization;

namespace SJInovacao.Acesso.Common.Security.Authentication
{
    public class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {         

            var permissions = context.User.FindAll("permissions").Select(p => p.Value).ToList();
            Console.WriteLine(string.Join(",", permissions));


            if (context.User?.Identity?.IsAuthenticated != true)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            //var permissions = context.User.FindAll("Permission").Select(c => c.Value).ToList();

            if (permissions.Contains(requirement.PermissionName))
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
