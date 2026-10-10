using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

public abstract class BaseRepository<TEntity> where TEntity : class
{
    protected readonly DefaultContext _context;
    protected readonly ILogger _logger;
    protected readonly string EntityName;

    protected BaseRepository(DefaultContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
        EntityName = typeof(TEntity).Name;
    }

    protected async Task<T> ExecuteWithLoggingAsync<T>(
        string operationName,
        Func<Task<T>> operation)
    {
        using (_logger.BeginScope(new Dictionary<string, object>
        {
            ["Entity"] = EntityName,
            ["Operation"] = operationName
        }))
        {
            var stopwatch = Stopwatch.StartNew();
            _logger.LogDebug("▶️ Iniciando {Operation} em {Entity}", operationName, EntityName);

            try
            {
                var result = await operation();
                stopwatch.Stop();

                _logger.LogInformation(
                    "✅ {Operation} em {Entity} | Duração: {Duration}ms",
                    operationName, EntityName, stopwatch.ElapsedMilliseconds);

                return result;
            }
            catch (Exception)
            {
                stopwatch.Stop();
                _logger.LogDebug(
                    "⏱️ {Operation} em {Entity} falhou após {Duration}ms",
                    operationName, EntityName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }

    protected Task ExecuteWithLoggingAsync(string operationName, Func<Task> operation)
        => ExecuteWithLoggingAsync<object?>(
            operationName,
            async () => { await operation(); return null; });
}