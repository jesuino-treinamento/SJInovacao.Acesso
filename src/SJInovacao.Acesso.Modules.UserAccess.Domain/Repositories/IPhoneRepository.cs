using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IPhoneRepository
    {
        Task<Phone?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
        Task<Phone> CreateAsync(Phone phone, CancellationToken cancellationToken);
        Task<Phone> UpdateAsync(Phone phone, CancellationToken cancellationToken);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken);
        Task<IEnumerable<Phone>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }

   
}