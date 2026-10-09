using Serilog.Context;
using System.Diagnostics;

namespace SJInovacao.Acesso.WebAPI.Middleware 
{

    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CorrelationIdMiddleware> _logger;
        private const string CorrelationIdHeader = "X-Correlation-ID";

        public CorrelationIdMiddleware(RequestDelegate next, ILogger<CorrelationIdMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var correlationId = context.Request.Headers.TryGetValue(CorrelationIdHeader, out var id)
                ? id.ToString()
                : Activity.Current?.Id ?? context.TraceIdentifier;

            using (LogContext.PushProperty("CorrelationId", correlationId))
            {
                // ✅ Mais seguro
                context.Response.Headers[CorrelationIdHeader] = correlationId;

                var stopwatch = Stopwatch.StartNew();
                _logger.LogInformation("➡️ {Method} {Path}", context.Request.Method, context.Request.Path);

                try
                {
                    await _next(context);
                }
                finally
                {
                    stopwatch.Stop();
                    _logger.LogInformation("⬅️ {Method} {Path} - {StatusCode} - {Duration}ms",
                        context.Request.Method, context.Request.Path, context.Response.StatusCode, stopwatch.ElapsedMilliseconds);
                }
            }
        }
    }
}