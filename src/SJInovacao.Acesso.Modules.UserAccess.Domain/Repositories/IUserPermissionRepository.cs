using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IUserPermissionRepository
    {
        Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct);
        Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct);
        Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct);
        Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct);
    }
}
