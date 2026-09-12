using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupPermissionUserRepository : IGroupPermissionUserRepository
    {
        private readonly DefaultContext _context; // substitua pelo seu DbContext

        public GroupPermissionUserRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<UserGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {           
            return await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(g => g.GroupId == id, cancellationToken);
        }

        public async Task<List<UserGroup>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<UserGroup>()
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        //public async Task<UserGroup> CreateAsync(GroupPermission groupUser, CancellationToken cancellationToken)
        //{
        //    var exists = await _context.GroupPermissions
        //    .AnyAsync(u => u.Name == groupUser.Name, cancellationToken);

        //    if (exists)
        //        throw new DomainException("A groupUser with the same name already exists");


        //    await _context.Set<UserGroup>().AddAsync(groupUser, cancellationToken);
        //    await _context.SaveChangesAsync(cancellationToken);
        //    return groupUser;
        //}

        public async Task UpdateAsync(UserGroup groupUser, CancellationToken cancellationToken = default)
        {
            _context.Set<UserGroup>().Update(groupUser);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await GetByIdAsync(id, cancellationToken);
            if (entity == null)
                return false;

            _context.Set<UserGroup>().Remove(entity);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            var junction = await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(j => j.GroupId == groupId && j.UserId == permissionId, ct);

            if (junction != null)
            {
                _context.Set<UserGroup>().Remove(junction);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task DeactivateAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            var userGroup = await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(g => g.GroupId == groupId && g.UserId == userId, cancellationToken);
            if (userGroup != null)
            {
                _context.Entry(userGroup).CurrentValues.SetValues(userGroup);
                userGroup.IsActive = false;
                userGroup.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task DeleteUserGroupAsync(Guid userId, Guid groupId)
        {
            var userGroup = await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(ug => ug.UserId == userId && ug.GroupId == groupId);

            if (userGroup != null)
            {
                _context.Set<UserGroup>().Remove(userGroup);
                await _context.SaveChangesAsync();
            }
        }
    }
}
