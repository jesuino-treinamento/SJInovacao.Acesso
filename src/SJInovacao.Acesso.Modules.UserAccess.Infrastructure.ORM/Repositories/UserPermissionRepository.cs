using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly DefaultContext _context;

        public UserPermissionRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            var entity = new UserPermission { UserId = userId, PermissionId = permissionId, IsActive = true };
            _context.UserPermissions.Add(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct)
        {
            var entity = await _context.UserPermissions.FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);
            if (entity != null)
            {
                entity.IsActive = status;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            var entity = await _context.UserPermissions.FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);
            if (entity != null)
            {
                _context.UserPermissions.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct)
            => await _context.UserPermissions.Where(up => up.UserId == userId).Select(up => up.Permission).ToListAsync(ct);

        public async Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
            => await _context.UserPermissions.Where(up => up.PermissionId == permissionId).Select(up => up.User).ToListAsync(ct);

        public async Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct)
            => await _context.Users
            .AsNoTracking()
            .Include(u => u.UserPermissions!)
                .ThenInclude(up => up.Permission)
            .Include(u => u.UserGroups)
            .ThenInclude(ug => ug.Group).ThenInclude(g => g.GroupsPermissions)
            .ThenInclude(gp => gp.Permission).ToListAsync(ct);

        public async Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct)
            => await _context.Users.Include(u => u.UserGroups).ThenInclude(ug => ug.Group).ThenInclude(g => g.GroupsPermissions).ThenInclude(gp => gp.Permission).FirstOrDefaultAsync(u => u.Id == userId, ct);

        public async Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId, CancellationToken ct)
            => await _context.UserPermissions.AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);
    }

}