using Microsoft.EntityFrameworkCore;

namespace SJInovacao.Acesso.Common.Auditing;

/// <summary>
/// Serviço responsável por capturar alterações de entidades IAuditable
/// no ChangeTracker do DbContext.
/// </summary>
public interface IAuditService
{
    List<AuditLog> CaptureChanges(DbContext context);
}