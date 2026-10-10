//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

//namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
//{
//    public interface IUsersGroupsPermissionsRepository
//    {
//        Task<UsersGroupsPermissions> CreateAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);
//        Task<bool> UpdateStatusAsync(Guid userId, Guid groupId, Guid permissionId, bool isActive, CancellationToken ct);
//        Task<bool> DeleteAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);
//        Task<IEnumerable<UsersGroupsPermissions>> GetByUserGroupAsync(Guid userId, Guid groupId, CancellationToken ct);
//        Task<IEnumerable<UsersGroupsPermissions>> GetAllAsync(CancellationToken ct);
//        Task UpdateAllUsersGroupsPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken);
//    }
//}

using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IUsersGroupsPermissionsRepository
    {
        // ===== Escrita =====
        Task<UsersGroupsPermissions> CreateAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);

        Task<bool> UpdateStatusAsync(
            Guid userId, Guid groupId, Guid permissionId, bool isActive, CancellationToken ct);

        Task<bool> DeleteAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);

        Task UpdateAllByPermissionIdAsync(
            Guid permissionId, bool isActive, CancellationToken ct);

        /// <summary>
        /// Atualiza o status do usuário no grupo e o status das permissões informadas.
        /// </summary>
        Task<IReadOnlyList<Permission>> UpdateUserGroupPermissionsAsync(
            Guid userId,
            Guid groupId,
            bool userIsActive,
            IEnumerable<Guid> permissionIds,
            bool? permissionIsActive,
            CancellationToken ct);

        // ===== Leitura =====
        Task<UsersGroupsPermissions?> GetAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct);

        Task<IReadOnlyList<UsersGroupsPermissions>> GetByUserGroupAsync(
            Guid userId, Guid groupId, CancellationToken ct);

        Task<IReadOnlyList<UsersGroupsPermissions>> GetAllAsync(
            int skip, int take, CancellationToken ct);

        Task<PaginatedList<UsersGroupsPermissions>> GetAllPaginatedAsync(
            int page, int size, string orderBy, CancellationToken ct);
    }
}