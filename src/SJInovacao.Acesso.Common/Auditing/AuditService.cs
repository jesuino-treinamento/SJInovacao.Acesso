using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SJInovacao.Acesso.Common.Security.Context;

namespace SJInovacao.Acesso.Common.Auditing;

/// <summary>
/// Serviço responsável por capturar alterações de entidades IAuditable
/// no ChangeTracker do DbContext e gerar registros de auditoria.
/// </summary>
public sealed class AuditService : IAuditService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = false,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    /// <summary>
    /// Propriedades que NÃO devem ter seus valores reais expostos.
    /// São mascaradas com "***" para manter rastreabilidade sem vazar dados.
    /// </summary>
    private static readonly HashSet<string> SensitiveProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "Password",
        "RefreshToken",
        "PasswordHash",
        "SecurityStamp",
        "ConcurrencyStamp"
    };

    /// <summary>
    /// Propriedades que devem ser completamente IGNORADAS na auditoria.
    /// </summary>
    private static readonly HashSet<string> IgnoredProperties = new(StringComparer.OrdinalIgnoreCase)
    {
        "RowVersion",
        "xmin"  // PostgreSQL system column
    };

    private readonly IUserContext _userContext;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuditService(IUserContext userContext, IHttpContextAccessor httpContextAccessor)
    {
        _userContext = userContext;
        _httpContextAccessor = httpContextAccessor;
    }

    public List<AuditLog> CaptureChanges(DbContext context)
    {
        context.ChangeTracker.DetectChanges();

        var auditLogs = new List<AuditLog>();
        var httpContext = _httpContextAccessor.HttpContext;

        foreach (var entry in context.ChangeTracker.Entries())
        {
            if (entry.Entity is not IAuditable) continue;
            if (entry.State is EntityState.Unchanged or EntityState.Detached) continue;

            // Captura valores comuns
            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityName = entry.Entity.GetType().Name,
                EntityId = GetPrimaryKeyValue(entry),
                Action = entry.State.ToString(),
                Timestamp = DateTime.UtcNow,
                UserId = ParseGuid(_userContext.UserId),
                UserName = _userContext.UserName,
                IpAddress = httpContext?.Connection.RemoteIpAddress?.ToString(),
                UserAgent = httpContext?.Request.Headers["User-Agent"].ToString(),
                CorrelationId = GetCorrelationId(httpContext)
            };

            switch (entry.State)
            {
                case EntityState.Added:
                    auditLog.NewValues = SerializeValues(entry.CurrentValues);
                    break;

                case EntityState.Deleted:
                    auditLog.OldValues = SerializeValues(entry.OriginalValues);
                    break;

                case EntityState.Modified:
                    var modified = GetModifiedProperties(entry);

                    // ✅ Se nada realmente mudou (todos os valores são idênticos), não loga
                    if (modified.Columns.Count == 0)
                        continue;

                    auditLog.OldValues = modified.OldValues;
                    auditLog.NewValues = modified.NewValues;
                    auditLog.AffectedColumns = string.Join(",", modified.Columns);
                    break;

                default:
                    continue;
            }

            auditLogs.Add(auditLog);
        }

        return auditLogs;
    }

    // =========================
    // Helpers
    // =========================

    private static Guid? ParseGuid(string? value)
        => Guid.TryParse(value, out var id) ? id : null;

    private static string GetPrimaryKeyValue(EntityEntry entry)
    {
        var pk = entry.Metadata.FindPrimaryKey();
        if (pk is null) return string.Empty;

        var values = pk.Properties
            .Select(p => entry.Property(p.Name).CurrentValue?.ToString() ?? string.Empty)
            .Where(v => !string.IsNullOrEmpty(v));

        return string.Join(",", values);
    }

    private static string SerializeValues(PropertyValues values)
    {
        var dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        foreach (var prop in values.Properties)
        {
            if (IgnoredProperties.Contains(prop.Name)) continue;

            var value = values[prop]?.ToString();

            if (SensitiveProperties.Contains(prop.Name))
                value = "***";

            dict[prop.Name] = value;
        }

        return JsonSerializer.Serialize(dict, JsonOptions);
    }

    /// <summary>
    /// Extrai as propriedades efetivamente modificadas.
    /// Ignora propriedades mascaradas e colunas de sistema.
    /// Retorna lista vazia se nada mudou de fato (values iguais).
    /// </summary>
    private static (string OldValues, string NewValues, List<string> Columns) GetModifiedProperties(EntityEntry entry)
    {
        var oldValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var newValues = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        var columns = new List<string>();

        foreach (var prop in entry.Properties)
        {
            if (!prop.IsModified) continue;
            if (IgnoredProperties.Contains(prop.Metadata.Name)) continue;

            var oldValue = prop.OriginalValue?.ToString();
            var newValue = prop.CurrentValue?.ToString();

            // ✅ Skip se o valor não mudou de fato (evita "Modified falso")
            if (oldValue == newValue)
                continue;

            var propName = prop.Metadata.Name;

            if (SensitiveProperties.Contains(propName))
            {
                oldValue = oldValue is null ? null : "***";
                newValue = newValue is null ? null : "***";
            }

            columns.Add(propName);
            oldValues[propName] = oldValue;
            newValues[propName] = newValue;
        }

        // Ordena alfabeticamente para ficar legível
        columns.Sort(StringComparer.OrdinalIgnoreCase);

        var orderedOld = oldValues.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);
        var orderedNew = newValues.OrderBy(kv => kv.Key).ToDictionary(kv => kv.Key, kv => kv.Value);

        return (
            JsonSerializer.Serialize(orderedOld, JsonOptions),
            JsonSerializer.Serialize(orderedNew, JsonOptions),
            columns
        );
    }

    private static string? GetCorrelationId(HttpContext? httpContext)
    {
        if (httpContext is null) return null;

        if (httpContext.Items.TryGetValue("CorrelationId", out var value) && value is not null)
            return value.ToString();

        // Fallback: usa o TraceIdentifier se o middleware não tiver rodado
        return httpContext.TraceIdentifier;
    }
}