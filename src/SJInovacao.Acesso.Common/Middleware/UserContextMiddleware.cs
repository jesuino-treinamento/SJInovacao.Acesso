using Microsoft.AspNetCore.Http;
using SJInovacao.Acesso.Common.Security.Context;
using System.Security.Claims;

namespace SJInovacao.Acesso.WebAPI.Middleware 
{
    public class UserContextMiddleware
    {
        private readonly RequestDelegate _next;

        public UserContextMiddleware(RequestDelegate next) => _next = next;

        public async Task InvokeAsync(HttpContext context, IUserContext userContext)
        {
            var user = context.User;

            if (user.Identity?.IsAuthenticated == true && userContext is UserContext concreteUserContext)
            {
                var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var userName = user.FindFirst(ClaimTypes.Name)?.Value;
                var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

                var permissions = user.FindAll("permissions").Select(p => p.Value).ToList();
                var groups = user.FindAll("groups").Select(p => p.Value).ToList();

                concreteUserContext.SetUserData(userId, userName, userRole, permissions, groups);
            }

            await _next(context);
        }
    }
}