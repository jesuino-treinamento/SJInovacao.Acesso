using Microsoft.OpenApi.Models;
using Serilog;
using SJInovacao.Acesso.Common.HealthChecks;
using SJInovacao.Acesso.Common.Logging;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.IoC;
using SJInovacao.Acesso.WebAPI.Common;
using SJInovacao.Acesso.WebAPI.Middleware;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        // ✅ Bootstrap logger (captura erros antes do host iniciar)
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            // ✅ ANTES do Build()
            builder.WebHost.CaptureStartupErrors(true);

            // =============================
            // 🔵 LOGGING + HEALTHCHECKS
            // =============================
            builder.AddDefaultLogging();
            builder.AddBasicHealthChecks();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();

            // =============================
            // 🔐 AUTHENTICATION (via extensão)
            // =============================
            builder.Services.AddJwtAuthentication(builder.Configuration);

            // =============================
            // 📦 SWAGGER + JWT BUTTON
            // =============================
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SJInovacao.Acesso.WebAPI",
                    Version = "v1"
                });

                var securityScheme = new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Description = "Digite: Bearer {seu_token}",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                var apiKeySecurityScheme = new OpenApiSecurityScheme
                {
                    Description = "Digite sua API Key",
                    Name = "X-API-KEY",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    }
                };

                c.AddSecurityDefinition("ApiKey", apiKeySecurityScheme);
                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { apiKeySecurityScheme, Array.Empty<string>() },
                    { securityScheme, Array.Empty<string>() }
                });
            });

            // =============================
            // 📦 IoC MÓDULOS
            // =============================

            builder.Services.AddHostedService<QueryWarmupService>();
            await builder.RegisterDependenciesAsync();

            var app = builder.Build();

            // Criar pasta de logs
            var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs");
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            // Validação de configuração
            var config = builder.Configuration;
            if (string.IsNullOrWhiteSpace(config["Jwt:SecretKey"]))
                throw new InvalidOperationException(
                    "Configuração ausente: 'Jwt:SecretKey'. Defina em appsettings.json ou variável de ambiente (Jwt__SecretKey).");

            // =============================
            // 🌐 PIPELINE DE MIDDLEWARES (ordem correta!)
            // =============================

            // 1. Rastreamento (mais cedo possível)
            app.UseMiddleware<CorrelationIdMiddleware>();

            // 2. Tratamento global de exceções
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var feature = context.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
                    var exception = feature?.Error;

                    var (status, message) = exception switch
                    {
                        UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Você não tem autorização para acessar este recurso."),
                        _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor.")
                    };

                    context.Response.StatusCode = status;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(JsonSerializer.Serialize(new { success = false, message }));
                });
            });

            // 3. Tratamento de status codes (401/403/404 do framework)
            app.UseStatusCodePages(async context =>
            {
                var response = context.HttpContext.Response;
                response.ContentType = "application/json";

                var message = response.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => "Você não tem autorização para acessar este recurso.",
                    StatusCodes.Status403Forbidden => "Você não tem permissão para acessar este recurso.",
                    StatusCodes.Status404NotFound => "Recurso não encontrado.",
                    _ => null
                };

                if (message != null)
                {
                    await response.WriteAsync(JsonSerializer.Serialize(new { success = false, message }));
                }
            });

            // 4. Swagger APENAS em Development
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            // 5. HTTPS Redirect (antes de auth)
            app.UseHttpsRedirection();

            // 6. Routing (explícito)
            app.UseRouting();

            // 7. Auth
            app.UseAuthentication();
            app.UseAuthorization();

            // 8. Middlewares de aplicação
            app.UseMiddleware<ValidationExceptionMiddleware>();
            app.UseMiddleware<UserContextMiddleware>();

            // 9. Health checks
            app.UseBasicHealthChecks();

            // 10. Endpoints
            app.MapControllers();

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}