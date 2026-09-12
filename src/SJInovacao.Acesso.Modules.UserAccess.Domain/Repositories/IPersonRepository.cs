using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IPersonRepository
    {
        Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Person?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken);
        Task<Person> CreateAsync(Person person, CancellationToken cancellationToken);
        Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken);
        Task<IEnumerable<Person>> GetAllAsync();
        Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken);
        Task<PaginatedList<Person>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}