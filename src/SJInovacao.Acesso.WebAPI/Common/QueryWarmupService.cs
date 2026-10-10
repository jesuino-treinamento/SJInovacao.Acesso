using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.WebAPI.Common;

public class QueryWarmupService : BackgroundService
{
    private static readonly TimeSpan StartupDelay = TimeSpan.FromSeconds(2);
    private static readonly TimeSpan WarmupTimeout = TimeSpan.FromSeconds(30);

    private readonly IServiceProvider _sp;
    private readonly ILogger<QueryWarmupService> _logger;

    public QueryWarmupService(IServiceProvider sp, ILogger<QueryWarmupService> logger)
    {
        _sp = sp;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timeoutCts = new CancellationTokenSource(WarmupTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken, timeoutCts.Token);
        var token = linkedCts.Token;

        try
        {
            // Aguarda a app terminar de subir
            await Task.Delay(StartupDelay, token);

            using var scope = _sp.CreateScope();
            var repo = scope.ServiceProvider
                .GetRequiredService<IGroupPermissionUserRepository>();

            _logger.LogInformation("🔥 Aquecendo queries em background...");

            await repo.GetAllPaginatedAsync(1, 1, "groupname asc", token);
            await repo.GetAllPaginatedAsync(1, 1, "groupname desc", token);
            await repo.GetAllPaginatedAsync(1, 1, "username asc", token);
            await repo.GetAllPaginatedAsync(1, 1, "username desc", token);
            await repo.GetAllPaginatedAsync(1, 1, "username asc, groupname desc", token);
            await repo.GetAllPaginatedAsync(1, 1, "username desc, groupname asc", token);

            await repo.GetAllWithFiltersAsync(false, null, 1, token);

            _logger.LogInformation("✅ Queries aquecidas");
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
        {
            _logger.LogWarning("⏱️ Warmup excedeu {Timeout}s", WarmupTimeout.TotalSeconds);
        }
        catch (OperationCanceledException)
        {
            // Shutdown solicitado — não é erro
            _logger.LogDebug("Warmup cancelado por shutdown");
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Warmup falhou — a aplicação continuará normalmente");
        }
    }
}