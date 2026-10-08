using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IUsersGroupsPermissionsRepository
    {
        Task<UsersGroupsPermissions> CreateAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);
        Task<bool> UpdateStatusAsync(Guid userId, Guid groupId, Guid permissionId, bool isActive, CancellationToken ct);
        Task<bool> DeleteAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);
        Task<IEnumerable<UsersGroupsPermissions>> GetByUserGroupAsync(Guid userId, Guid groupId, CancellationToken ct);
        Task<IEnumerable<UsersGroupsPermissions>> GetAllAsync(CancellationToken ct);
        Task UpdateAllUsersGroupsPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken);
    }
}
