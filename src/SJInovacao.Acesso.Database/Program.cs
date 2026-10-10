using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using SJInovacao.Acesso.Common.HealthChecks;
using SJInovacao.Acesso.Common.Logging;
using SJInovacao.Acesso.Common.Security;
using SJInovacao.Acesso.IoC;
using SJInovacao.Acesso.WebAPI.Middleware;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using System.Text.Json;

public class Program
{
    public static async Task Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting web application");

            var builder = WebApplication.CreateBuilder(args);

            builder.WebHost.CaptureStartupErrors(true);

            builder.AddDefaultLogging();
            builder.AddBasicHealthChecks();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddControllers();

            builder.Services.AddJwtAuthentication(builder.Configuration);

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

            await builder.RegisterDependenciesAsync();

            var app = builder.Build();

            var logsPath = Path.Combine(builder.Environment.ContentRootPath, "logs");
            if (!Directory.Exists(logsPath))
                Directory.CreateDirectory(logsPath);

            var config = builder.Configuration;
            if (string.IsNullOrWhiteSpace(config["Jwt:SecretKey"]))
                throw new InvalidOperationException(
                    "Configuração ausente: 'Jwt:SecretKey'. Defina em appsettings.json ou variável de ambiente (Jwt__SecretKey).");

            //app.UseMiddleware<CorrelationIdMiddleware>();

            // =============================
            // 2. Tratamento global de exceções
            // =============================
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var feature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = feature?.Error;

                    var (status, message) = exception switch
                    {
                        // FluentValidation → 400 (caso não seja capturado pelo middleware específico)
                        FluentValidation.ValidationException => (
                            StatusCodes.Status400BadRequest,
                            "Erro de validação nos dados enviados."),

                        // DomainException → 400 (regra de negócio violada)
                        SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions.DomainException domainEx => (
                            StatusCodes.Status400BadRequest,
                            domainEx.Message),

                        // Not found → 404 (adicione se você tiver essa exception)
                        // NotFoundException => (404, "Recurso não encontrado."),

                        // Auth → 401
                        UnauthorizedAccessException => (
                            StatusCodes.Status401Unauthorized,
                            "Você não tem autorização para acessar este recurso."),

                        // Qualquer outra exceção → 500 (não vaza detalhes internos)
                        _ => (
                            StatusCodes.Status500InternalServerError,
                            "Erro interno do servidor. Contate o suporte.")
                    };

                    // Loga o erro real (o cliente não vê os detalhes)
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                    logger.LogError(exception,
                        "Erro não tratado em {Method} {Path}",
                        context.Request.Method,
                        context.Request.Path);

                    context.Response.StatusCode = status;
                    context.Response.ContentType = "application/json";

                    await context.Response.WriteAsync(JsonSerializer.Serialize(new
                    {
                        success = false,
                        message
                    }));
                });
            });

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

            // 🔐 PROTEÇÃO DO SWAGGER EM PRODUÇÃO
            //if (app.Environment.IsProduction())                       
            //{
            //    app.UseMiddleware<SwaggerBasicAuthMiddleware>();
            //}

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            //app.UseMiddleware<ValidationExceptionMiddleware>();
            app.UseMiddleware<UserContextMiddleware>();

            app.UseBasicHealthChecks();

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