using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class PermissionRepository : IPermissionRepository
    {
        private readonly DefaultContext _context;

        public PermissionRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.Permissions
                .Include(p => p.Groups)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Permissions
                .Include(p => p.Groups)
                .ToListAsync(cancellationToken);
        }

        public async Task<Permission> CreateAsync(Permission permission, CancellationToken cancellationToken)
        {
            var exists = await _context.Permissions
            .AnyAsync(u => u.Name == permission.Name, cancellationToken);

            if (exists)
                throw new DomainException("A permission with the same name already exists");

            await _context.Permissions.AddAsync(permission, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return permission;
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exists = await GetByIdAsync(id, cancellationToken);
            if (exists == null)
                return false;

            _context.Entry(exists).CurrentValues.SetValues(exists);
            exists.Deactivate();
            exists.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            //return existingUser;
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var exists = await GetByIdAsync(id, cancellationToken);
            if (exists == null)
                return false;

            _context.Permissions.Remove(exists);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public void Remove(Permission permission)
        {
            _context.Permissions.Remove(permission);
        }

        public async Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            permission.UpdatedAt = DateTime.UtcNow;
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync(cancellationToken);
            return permission;
        }

        public async Task<bool> GetNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var exists = await _context.Permissions.AnyAsync(u => u.Name == name, cancellationToken);
            if (exists)
                return false;

            return true;
        }
    }
}
