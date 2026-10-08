using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupPermissionRepository
    {
        Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<GroupPermission> CreateAsync(GroupPermission grouppPermission, CancellationToken cancellationToken);
        Task<GroupPermission> UpdateAsync(GroupPermission groupUser, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> GroupNameExistsAsync(string name, CancellationToken cancellationToken);
        public Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken);
        Task<IEnumerable<GroupPermission>> GetAllWithGroupPermissionsAsync(CancellationToken ct);
        Task<IEnumerable<Permission>> GetByGroupIdAsync(Guid groupId, CancellationToken ct);
        Task RemoveAsync(Guid groupId, CancellationToken ct);        
    }
}
