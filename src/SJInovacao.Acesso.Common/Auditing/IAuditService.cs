using Microsoft.EntityFrameworkCore;

namespace SJInovacao.Acesso.Common.Auditing;

public interface IAuditService
{
    List<AuditLog> CaptureChanges(DbContext context);
}