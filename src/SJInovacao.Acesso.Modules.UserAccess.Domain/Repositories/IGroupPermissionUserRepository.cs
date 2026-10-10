using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Projections;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupPermissionUserRepository
    {
        Task<UserGroup?> GetByIdAsync(Guid userId, Guid groupId, CancellationToken ct);

        Task<PaginatedList<UserGroupSummary>> GetAllPaginatedAsync(
            int page, int size, string? orderBy, CancellationToken ct);

        Task<List<UserGroupWithPermissionsSummary>> GetAllWithFiltersAsync(
            bool onlyActiveUsers,
            string? permissionNameFilter,
            int? maxResults,
            CancellationToken ct);

        Task<Dictionary<(Guid UserId, Guid GroupId), List<Permission>>> GetPermissionsForPageAsync(
            IReadOnlyList<Guid> userIds, IReadOnlyList<Guid> groupIds, CancellationToken ct);

        Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct);
    }
}