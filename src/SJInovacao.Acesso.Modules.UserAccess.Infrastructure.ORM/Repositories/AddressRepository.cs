using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class AddressRepository : IAddressRepository
    {
        private readonly DefaultContext _context;
        private readonly ILogger<AddressRepository> _logger;

        public AddressRepository(DefaultContext context, ILogger<AddressRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Address>()
                .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
        }

        public async Task<Address> CreateAsync(Address address, CancellationToken cancellationToken)
        {
            await _context.Set<Address>().AddAsync(address, cancellationToken);
            return address;
        }

        public async Task<Address> UpdateAsync(Address address, CancellationToken cancellationToken)
        {
            _context.Set<Address>().Update(address);
            return await Task.FromResult(address);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var address = await GetByIdAsync(id, cancellationToken);
            if (address == null)
                return false;

            address.Deactivate();
            _context.Set<Address>().Update(address);
            return true;
        }

        public async Task<IEnumerable<Address>> GetByPersonIdAsync(Guid personId, CancellationToken cancellationToken)
        {
            return await _context.Set<Address>()
                .Where(a => a.PersonId == personId && a.IsActive)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<IEnumerable<Address>> GetAllAsync()
        {
            try
            {
                return await _context.Addresses
                    .Include(a => a.Person)
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all addresses");
                throw;
            }
        }
    }
}
