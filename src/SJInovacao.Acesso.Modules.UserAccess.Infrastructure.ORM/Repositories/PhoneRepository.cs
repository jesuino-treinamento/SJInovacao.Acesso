using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class PhoneRepository : IPhoneRepository
    {
        private readonly DefaultContext _context;

        public PhoneRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Phone?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Phone>()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Phone> CreateAsync(Phone phone, CancellationToken cancellationToken)
        {
            await _context.Set<Phone>().AddAsync(phone, cancellationToken);
            return phone;
        }

        public async Task<Phone> UpdateAsync(Phone phone, CancellationToken cancellationToken)
        {
            _context.Set<Phone>().Update(phone);
            return await Task.FromResult(phone);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var phone = await GetByIdAsync(id, cancellationToken);
            if (phone == null)
                return false;

            phone.Deactivate();
            _context.Set<Phone>().Update(phone);
            return true;
        }

        public async Task<IEnumerable<Phone>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken)
        {
            return await _context.Set<Phone>()
                .Where(p => p.PersonId == personId && p.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}