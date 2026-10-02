using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupPermissionUserRepository
    {
        Task DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct);
        Task<UserGroup?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<List<UserGroup>> GetAllAsync(CancellationToken cancellationToken);
        Task<PaginatedList<UserGroup>> GetAllPaginatedAsync(int page, int size, string orderBy, CancellationToken cancellationToken);
        IQueryable<UserGroup> Query();
    }    
}
