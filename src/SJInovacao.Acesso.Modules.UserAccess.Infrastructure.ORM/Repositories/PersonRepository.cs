using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly DefaultContext _context;

        public PersonRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Person?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Person>()
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Person?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Person>()
                .Include(p => p.Phones.Where(ph => ph.IsActive))
                .Include(p => p.Addresses.Where(a => a.IsActive))
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Person> CreateAsync(Person person, CancellationToken cancellationToken)
        {
            await _context.Set<Person>().AddAsync(person, cancellationToken);
            return person;
        }

        public async Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken)
        {
            _context.Set<Person>().Update(person);
            return await Task.FromResult(person);
        }

        public async Task<IEnumerable<Person>> GetAllAsync()
        {
            return await _context.Set<Person>()
                .Include(p => p.Phones)
                .Include(p => p.Addresses)
                .ToListAsync();
        }

        public async Task<IEnumerable<Person>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<Person>()
                .Include(p => p.Phones)
                .Include(p => p.Addresses)
                .ToListAsync(cancellationToken);
        }

        public async Task<PaginatedList<Person>> GetPagedAsync(int pageNumber, int pageSize, string? searchTerm, CancellationToken cancellationToken)
        {
            var query = _context.Set<Person>()
                .Include(p => p.Phones)
                .Include(p => p.Addresses)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Name.FirstName.Contains(searchTerm) ||
                    p.Name.LastName.Contains(searchTerm) ||
                    p.Document.Number.Contains(searchTerm));
            }

            return PaginatedList<Person>.Create(query, pageNumber, pageSize);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var person = await GetByIdAsync(id, cancellationToken);
            if (person == null)
                return false;

            _context.Set<Person>().Remove(person);
            return true;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Set<Person>().AnyAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}