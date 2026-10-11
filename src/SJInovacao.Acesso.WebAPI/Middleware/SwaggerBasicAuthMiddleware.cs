using System.Text;

namespace SJInovacao.Acesso.WebAPI.Middleware 
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
            if (context.Request.Path.StartsWithSegments("/swagger"))
            {
                string authHeader = context.Request.Headers["Authorization"].ToString();
                if (authHeader != null && authHeader.StartsWith("Basic "))
                {
                    var encodedUsernamePassword = authHeader.Substring("Basic ".Length).Trim();
                    var encoding = Encoding.GetEncoding("iso-8859-1");
                    var usernamePassword = encoding.GetString(Convert.FromBase64String(encodedUsernamePassword));

                    var expectedUsername = _configuration["SwaggerAuth:Username"];
                    var expectedPassword = _configuration["SwaggerAuth:Password"];

                    if (usernamePassword == $"{expectedUsername}:{expectedPassword}")
                    {
                        await _next.Invoke(context);
                        return;
                    }
                }

                context.Response.StatusCode = 401;
                context.Response.Headers["WWW-Authenticate"] = "Basic realm=\"Swagger\"";
                return;
            }

            await _next.Invoke(context);
        }
    }
}