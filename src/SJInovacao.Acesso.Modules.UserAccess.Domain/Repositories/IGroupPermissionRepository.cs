using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupPermissionRepository
    {
        Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<List<GroupPermission>> GetAllAsync(CancellationToken cancellationToken);
        Task<GroupPermission> CreateAsync(GroupPermission groupUser, CancellationToken cancellationToken);
        Task<GroupPermission> UpdateAsync(GroupPermission groupUser, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> GroupNameExistsAsync(string name, Guid id, CancellationToken cancellationToken);
        public Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
    }
}
