# ADR-003 — BaseRepository com ExecuteWithLoggingAsync

- **Status:** Aceito
- **Data:** 04/10/2026
- **Decisores:** Tech Lead, Dev Backend

## Contexto

Com **9 módulos planejados** e múltiplos repositórios por módulo, sem padronização:

- Logs ficam inconsistentes entre repositórios
- Medição de performance é manual e replicada
- Cada repositório trata erros de forma diferente
- Auditoria é difícil de integrar

## Decisão

Criar `BaseRepository<TEntity>` abstrato com:

1. **Injeção de `DefaultContext` e `ILogger`**
2. **Método `ExecuteWithLoggingAsync`** que:
   - Cria scope com `Entity` e `Operation` (structured logging)
   - Mede duração com `Stopwatch`
   - Loga início e fim
   - Propaga exceções sem logar duplicado (middleware global cuida)
3. **Padronização para escritas:**

   ```csharp
   public Task<Permission> CreateAsync(Permission entity, CancellationToken ct)
       => ExecuteWithLoggingAsync(
           $"Create:{entity.Name}",
           async () =>
           {
               // validações de negócio
               // DomainException em falhas
               _context.Permissions.Add(entity);
               await _context.SaveChangesAsync(ct);
               return entity;
           });
   ```

## Consequências

### Positivas

- ✅ **Consistência:** todo repositório loga da mesma forma
- ✅ **Observabilidade:** duração por operação disponível em todos os endpoints
- ✅ **Manutenção:** corrigir bug em um lugar reflete em todos
- ✅ **Onboarding:** novo dev copia o padrão do vizinho
- ✅ **Zero log duplicado:** middleware global é o único responsável por logar exceções

### Negativas

- ⚠️ Ligeiro overhead de `Stopwatch` por chamada (~microsegundos)
- ⚠️ Requer disciplina para não misturar patterns

### Mitigações

- Overhead insignificante comparado ao custo do I/O
- Code review valida aderência ao padrão

## Estrutura

```csharp
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
            catch
            {
                stopwatch.Stop();
                _logger.LogDebug(
                    "⏱️ {Operation} em {Entity} falhou após {Duration}ms",
                    operationName, EntityName, stopwatch.ElapsedMilliseconds);
                throw;
            }
        }
    }
}
```

## Referências

- [BaseRepository.cs](../../src/SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM/Repositories/BaseRepository.cs)
- [Backlog US01](../backlog/us01-backlog.md)