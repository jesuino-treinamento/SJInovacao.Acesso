using System.Security.Claims;
using SJInovacao.Acesso.Common.Security.Context;
using Microsoft.AspNetCore.Http;

namespace SJInovacao.Acesso.Common.Middleware
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

                concreteUserContext.SetUserData(userId, userName, userRole, permissions);
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Você não tem permissão para acessar este recurso."
                }));
            }

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Você não tem autorização para acessar este recurso."
                }));
            }

            await _next(context);
        }
    }
}
