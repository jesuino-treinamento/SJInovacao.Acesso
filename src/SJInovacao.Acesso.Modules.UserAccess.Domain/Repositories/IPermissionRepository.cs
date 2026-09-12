using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IPermissionRepository
    {
        Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken);
        Task<Permission> CreateAsync(Permission permission, CancellationToken cancellationToken);
        Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> GetNameAsync(string name, CancellationToken cancellationToken = default);
    }
}
