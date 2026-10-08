using Microsoft.Extensions.Configuration;
using System.Text;

namespace SJInovacao.Acesso.WebAPI.Middleware // Ajuste o namespace conforme sua estrutura
{
    public class SwaggerBasicAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public SwaggerBasicAuthMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Verifica se a requisição é para a rota do Swagger
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"];
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    // Obtém as credenciais codificadas em Base64
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    // Lê as credenciais configuradas no appsettings
                    var expectedUsername = _configuration["SwaggerAuth:Username"];
                    var expectedPassword = _configuration["SwaggerAuth:Password"];

                    if (usernamePassword == $"{expectedUsername}:{expectedPassword}")
                    {
                        await _next.Invoke(context);
                        return;
                    }
                }

                // Se a autenticação falhar, retorna 401 e o cabeçalho para o navegador exibir o prompt
                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
                return;
            }

            await _next.Invoke(context);
        }
    }
}