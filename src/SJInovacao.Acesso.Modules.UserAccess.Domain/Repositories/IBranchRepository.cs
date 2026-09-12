using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IBranchRepository
    {
        Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Address> CreateAsync(Address address, CancellationToken cancellationToken);
        Task<Address> UpdateAsync(Address address, CancellationToken cancellationToken);
        Task<IEnumerable<Address>> GetAllAsync();
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
    }
}
