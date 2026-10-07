using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
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
            => await _context.Permissions.FindAsync(new object[] { id }, cancellationToken);

        public async Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken)
            => await _context.Permissions
            .Include(p => p.UsersGroupsPermissions)
                .ThenInclude(g => g.User)
            .ToListAsync(cancellationToken);

        public async Task<Permission> CreateAsync(Permission permission, CancellationToken cancellationToken)
        {
            var exist = await GetNameAsync(permission.Name, cancellationToken);
            if (exist)
                throw new DomainException("A permission with the same name already exists");

            _context.Permissions.Add(permission);
            await _context.SaveChangesAsync(cancellationToken);
            return permission;
        }

        public async Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            _context.Permissions.Update(permission);
            await _context.SaveChangesAsync(cancellationToken);
            return permission;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;
            _context.Permissions.Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null) return false;
            entity.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> GetNameAsync(string name, CancellationToken cancellationToken = default)
            => await _context.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
    }
}

