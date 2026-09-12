using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<User> CreateAsync(User user, CancellationToken cancellationToken = default);
        Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default);
        Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default);
        Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default);
        Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy);
        Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy);
        IQueryable<User> Query();

        Task<User?> GetGroupToUserAsync(Guid userId, CancellationToken cancellationToken);
        Task<User> RemoveGroupFromUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default);

        Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken);
        Task<bool> GetGroupNameAsync(string name, CancellationToken cancellationToken);

        Task<(List<User> users, int totalCount)> GetAllPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool sortDescending,
        string searchTerm,
        CancellationToken cancellationToken);

        Task<User?> GetByEmailWithPermissionsAndGroupsAsync(string email, CancellationToken cancellationToken);
        Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
        Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken);
        Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);
    }
}
