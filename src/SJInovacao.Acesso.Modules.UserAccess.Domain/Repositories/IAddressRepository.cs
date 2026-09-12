using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IAddressRepository
    {
        Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Address> CreateAsync(Address address, CancellationToken cancellationToken);
        Task<Address> UpdateAsync(Address address, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Address>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
