using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System.Diagnostics;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

/// <summary>
/// Classe base para todos os repositórios com logging estruturado
/// </summary>
public abstract class BaseRepository<TEntity> where TEntity : class
{
    protected readonly DefaultContext Context;
    protected readonly ILogger Logger;
    protected readonly string EntityName;

    protected BaseRepository(DefaultContext context, ILogger logger)
    {
        Context = context;
        Logger = logger;
        EntityName = typeof(TEntity).Name;
    }

    /// <summary>
    /// Log estruturado com timing automático
    /// </summary>
    protected async Task<T> ExecuteWithLoggingAsync<T>(
        string operationName,
        Func<Task<T>> operation)
    {
        using (LogContext.PushProperty("Entity", EntityName))
        using (LogContext.PushProperty("Operation", operationName))
        {
            var stopwatch = Stopwatch.StartNew();           
            try
            {
                Logger.LogInformation(
                    "▶️ Starting {Operation} on {Entity}",
                    operationName,
                    EntityName);

                var result = await operation();

                stopwatch.Stop();

                Logger.LogInformation(
                    "✅ Completed {Operation} on {Entity} | Duration: {Duration}ms",
                    operationName,
                    EntityName,
                    stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Logger.LogError(ex,
                    "❌ Failed {Operation} on {Entity} | Duration: {Duration}ms | Error: {Error}",
                    operationName,
                    EntityName,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }
    }

    /// <summary>
    /// Versão void do execute com logging
    /// </summary>
    protected async Task ExecuteWithLoggingAsync(
        string operationName,
        Func<Task> operation)
    {
        using (LogContext.PushProperty("Entity", EntityName))
        using (LogContext.PushProperty("Operation", operationName))
        {
            var stopwatch = Stopwatch.StartNew();

            try
            {
                Logger.LogInformation(
                    "▶️ Starting {Operation} on {Entity}",
                    operationName,
                    EntityName);

                await operation();

                stopwatch.Stop();

                Logger.LogInformation(
                    "✅ Completed {Operation} on {Entity} | Duration: {Duration}ms",
                    operationName,
                    EntityName,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                Logger.LogError(ex,
                    "❌ Failed {Operation} on {Entity} | Duration: {Duration}ms | Error: {Error}",
                    operationName,
                    EntityName,
                    stopwatch.ElapsedMilliseconds,
                    ex.Message);

                throw;
            }
        }
    }
}