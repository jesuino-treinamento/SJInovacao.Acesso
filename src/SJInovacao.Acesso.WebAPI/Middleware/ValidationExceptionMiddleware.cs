using FluentValidation;
using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.WebAPI.Common;
using System.Security.Claims;
using System.Text.Json;

namespace SJInovacao.Acesso.WebAPI.Middleware
{
    public class ValidationExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ValidationExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);

                var user = context.User;

                if (user.Identity?.IsAuthenticated == true)
                {
                    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    var userName = user.FindFirst(ClaimTypes.Name)?.Value;
                    var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

                    context.Items["UserId"] = userId;
                    context.Items["UserName"] = userName;
                    context.Items["UserRole"] = userRole;

                    var permissions = user.FindAll("permissions").Select(p => p.Value).ToList();
                    Console.WriteLine("Permissões autenticadas: " + string.Join(", ", permissions));
                    
                    context.Items["Permissions"] = permissions;

                    var groups = user.FindAll("groups").Select(p => p.Value).ToList();
                    Console.WriteLine("Groups autenticadas: " + string.Join(", ", groups));

                    context.Items["Groups"] = groups;
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
                else if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(new
                    {
                        success = false,
                        message = "Você não tem permissão para acessar este recurso."
                    }));
                }



            }
            catch (ValidationException ex)
            {
                await HandleValidationExceptionAsync(context, ex);
            }
        }

        private static Task HandleValidationExceptionAsync(HttpContext context, ValidationException exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status400BadRequest;

            var response = new ApiResponse
            {
                Success = false,
                Message = "Validation Failed",
                Errors = exception.Errors
                    .Select(error => (ValidationErrorDetail)error)
            };

            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
        }
    }
}
