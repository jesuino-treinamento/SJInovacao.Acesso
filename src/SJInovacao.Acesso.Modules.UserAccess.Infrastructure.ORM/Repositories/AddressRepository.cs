//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;

//namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
//{
//    public class AddressRepository : IAddressRepository
//    {
//        private readonly DefaultContext _context;
//        private readonly ILogger<AddressRepository> _logger;

//        public AddressRepository(DefaultContext context, ILogger<AddressRepository> logger)
//        {
//            _context = context;
//            _logger = logger;
//        }

//        public async Task<Address> CreateAsync(Address address, CancellationToken cancellationToken)
//        {
//            try
//            {
//                await _context.Addresses.AddAsync(address, cancellationToken);
//                await _context.SaveChangesAsync(cancellationToken);
//                _logger.LogInformation($"Address created with ID {address.Id}");
//                return address;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error creating address");
//                throw;
//            }
//        }

//        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var address = await _context.Addresses.FindAsync(new object[] { id }, cancellationToken);
//                if (address == null)
//                    return false;

//                _context.Addresses.Remove(address);
//                await _context.SaveChangesAsync(cancellationToken);
//                _logger.LogInformation($"Address deleted with ID {id}");
//                return true;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error deleting address with ID {id}");
//                throw;
//            }
//        }

//        public async Task<IEnumerable<Address>> GetAllAsync()
//        {
//            try
//            {
//                return await _context.Addresses
//                    .Include(a => a.Person)
//                    .AsNoTracking()
//                    .ToListAsync();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving all addresses");
//                throw;
//            }
//        }

//        public async Task<Address?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
//        {
//            try
//            {
//                return await _context.Addresses
//                    .Include(a => a.Person)
//                    .AsNoTracking()
//                    .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error retrieving address with ID {id}");
//                throw;
//            }
//        }

//        public async Task<Address> UpdateAsync(Address address, CancellationToken cancellationToken)
//        {
//            try
//            {
//                var existingAddress = await _context.Addresses.FindAsync(new object[] { address.Id }, cancellationToken);
//                if (existingAddress == null)
//                    throw new InvalidOperationException($"Address with ID {address.Id} not found");

//                _context.Entry(existingAddress).CurrentValues.SetValues(address);
//                await _context.SaveChangesAsync(cancellationToken);
//                _logger.LogInformation($"Address updated with ID {address.Id}");
//                return address;
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, $"Error updating address with ID {address.Id}");
//                throw;
//            }
//        }
//    }
//}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

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
