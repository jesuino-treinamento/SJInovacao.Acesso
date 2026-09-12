using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupPermissionRepository : IGroupPermissionRepository
    {
        private readonly DefaultContext _context;

        public GroupPermissionRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            return await _context.GroupPermissions
                .Include(g => g.Users)
                .Include(g => g.Permissions)
                .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
        }

        public async Task<List<GroupPermission>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.GroupPermissions
                .Include(g => g.Users)
                .Include(g => g.Permissions)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
        {
            return await _context.Permissions
                .Where(p => ids.Contains(p.Id) && p.IsActive)
                .ToListAsync(cancellationToken);
        }


        public async Task<GroupPermission> CreateAsync(GroupPermission groupPermission, CancellationToken cancellationToken)
        {
            ////Erro: An error occurred while saving the entity changes. See the inner exception for details.

            ////Opção 1 com erro
            //var exists = await _context.GroupPermissions
            //.AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);

            //if (exists)
            //    throw new DomainException("A groupUser with the same name already exists");

            //await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
            //await _context.SaveChangesAsync(cancellationToken);
            //return groupPermission;


            ////Opção 2 com erro
            //var exists = await _context.GroupPermissions
            //    .AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);
            //if (exists)
            //    throw new DomainException("A group with the same name already exists");

            //// Anexa permissões existentes (já devem estar no contexto ou serão anexadas)
            //foreach (var permission in groupPermission.Permissions)
            //{
            //    if (_context.Entry(permission).State == EntityState.Detached)
            //        _context.Entry(permission).State = EntityState.Unchanged;
            //}

            //await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
            //await _context.SaveChangesAsync(cancellationToken);
            //return groupPermission;

            try
            {
                var exists = await _context.GroupPermissions
                    .AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);
                if (exists)
                    throw new DomainException("A group with the same name already exists");

                foreach (var permission in groupPermission.Permissions)
                {
                    if (_context.Entry(permission).State == EntityState.Detached)
                        _context.Entry(permission).State = EntityState.Unchanged;
                }

                await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);
                return groupPermission;
            }
            catch (DbUpdateException ex)
            {
                // Isso vai mostrar o erro real do banco
                var innerMessage = ex.InnerException?.Message;
                throw new Exception($"Erro no banco: {innerMessage}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            //_context.GroupUsers.Remove(groupuser);
            //await _context.SaveChangesAsync(cancellationToken);

            var groupPermission = await GetByIdAsync(id, cancellationToken);
            if (groupPermission == null)
                return false;

            _context.GroupPermissions.Remove(groupPermission);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public void Remove(GroupPermission groupPermission)
        {
            _context.GroupPermissions.Remove(groupPermission);
        }

        public async Task<GroupPermission> UpdateAsync(GroupPermission groupPermission, CancellationToken cancellationToken = default)
        {
            _context.GroupPermissions.Update(groupPermission);
            await _context.SaveChangesAsync(cancellationToken);

            return groupPermission;
        }

        public async Task<bool> GroupNameExistsAsync(string name, Guid excludeGroupId, CancellationToken cancellationToken)
        {
            // Verifica se o novo nome já existe (excluindo o próprio grupo)
            var lowerName = name.ToLowerInvariant();
            return await _context.GroupPermissions
                .AnyAsync(g => g.Name.ToLower() == lowerName && g.Id != excludeGroupId, cancellationToken);
        }
    }
}
