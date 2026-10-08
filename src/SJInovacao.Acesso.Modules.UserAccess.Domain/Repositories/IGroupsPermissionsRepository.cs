using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupsPermissionsRepository
    {
        Task<GroupsPermissions?> GetByIdAsync(Guid groupId, Guid permissionId, CancellationToken ct);
        Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct);
        Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct);
        Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct);
        Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct);
        Task<IEnumerable<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct);
        Task<IEnumerable<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct);
        Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct);
        Task UpdateAllGroupsPermissionStatusAsync(Guid permissionId, bool isActive, CancellationToken cancellationToken);
    }
}
