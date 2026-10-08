using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Mime;
namespace SJInovacao.Acesso.Common.HealthChecks
{
    public static class HealthChecksExtension
    {
        public static void AddBasicHealthChecks(this WebApplicationBuilder builder)
        {
            builder.Services.AddHealthChecks()
                .AddCheck("Liveness", () => HealthCheckResult.Healthy(), tags: ["liveness"])
                .AddCheck("Readiness", () => HealthCheckResult.Healthy(), tags: ["readiness"]);
        }

        public static IServiceCollection AddAdvancedHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                // ===== CORE HEALTH CHECKS =====
                .AddCheck("Self", () => HealthCheckResult.Healthy("API is running"))

                // ===== DATABASE =====
               // .AddCheck<DatabaseHealthCheck>("Database", HealthStatus.Unhealthy, new[] { "database" })

                // ===== MEMORY =====
                .AddCheck("Memory", () =>
                {
                    var totalMemory = GC.GetTotalMemory(false);
                    const long threshold = 500 * 1024 * 1024; // 500MB

                    if (totalMemory > threshold)
                    {
                        return HealthCheckResult.Degraded($"High memory usage: {totalMemory / 1024 / 1024}MB");
                    }

                    return HealthCheckResult.Healthy($"Memory usage: {totalMemory / 1024 / 1024}MB");
                })

                // ===== STARTUP TIME =====
                .AddCheck("Startup", () =>
                {
                    var uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();

                    if (uptime.TotalSeconds < 5)
                    {
                        return HealthCheckResult.Degraded($"Application just started: {uptime.TotalSeconds:F2}s ago");
                    }

                    return HealthCheckResult.Healthy($"Uptime: {uptime.TotalSeconds:F2}s");
                });

            return services;
        }

       public static void UseBasicHealthChecks(this WebApplication app)
        {
            var livenessOptions = WriteHealtCheckRespose(app, "liveness");
            app.UseHealthChecks("/health/live", livenessOptions);

            var readinessOptions = WriteHealtCheckRespose(app, "readiness");
            app.UseHealthChecks("/health/ready", readinessOptions);

            var healthOptions = WriteHealtCheckRespose(app, string.Empty);
            app.UseHealthChecks("/health", healthOptions);

            var logger = app.Services.GetRequiredService<ILogger<HealthCheckService>>();
            logger.LogInformation("Health Check enabled at: '/health'");
        }

        public static WebApplication UseAdvancedHealthChecks(this WebApplication app)
        {
            app.MapHealthChecks("/health", new()
            {
                ResponseWriter = WriteResponse
            });

            app.MapHealthChecks("/health/ready", new()
            {
                Predicate = check => check.Tags.Contains("database")
            });

            return app;
        }
        private static async Task WriteResponse(HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var response = new
            {
                status = report.Status.ToString(),
                timestamp = DateTime.UtcNow,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    description = e.Value.Description,
                    duration = e.Value.Duration.TotalMilliseconds
                })
            };

            await context.Response.WriteAsJsonAsync(response);
        }

        private static HealthCheckOptions WriteHealtCheckRespose(this WebApplication app, string tag)
        {
            var options = new HealthCheckOptions
            {
                Predicate = (check) => check.Tags.Contains(tag),
                ResultStatusCodes =
            {
                [HealthStatus.Healthy] = StatusCodes.Status200OK,
                [HealthStatus.Degraded] = StatusCodes.Status200OK,
                [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
            },
                ResponseWriter = async (context, report) =>
                {
                    var result = new
                    {
                        status = report.Status.ToString(),
                        healthChecks = report.Entries.Select(e => new
                        {
                            name = e.Key,
                            status = e.Value.Status.ToString(),
                            description = e.Value.Description,
                            errorMessage = e.Value.Exception?.Message,
                            hostEnvironment = app.Environment.EnvironmentName.ToLowerInvariant()
                        }),
                    };
                    context.Response.ContentType = MediaTypeNames.Application.Json;
                    await context.Response.WriteAsJsonAsync(result);
                },
            };

            return options;
        }
    }
}
