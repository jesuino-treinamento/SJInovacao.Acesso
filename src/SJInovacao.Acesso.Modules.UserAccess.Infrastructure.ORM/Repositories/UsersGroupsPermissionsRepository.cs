using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UsersGroupsPermissionsRepository : IUsersGroupsPermissionsRepository
    {
        private readonly DefaultContext _context;

        public UsersGroupsPermissionsRepository(DefaultContext context)
        {
            _context = context;
        }

        // Criar vínculo usuário ↔ grupo ↔ permissão
        public async Task<UsersGroupsPermissions> CreateAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct)
        {
            var entity = new UsersGroupsPermissions
            {
                UserId = userId,
                GroupId = groupId,
                PermissionId = permissionId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.UsersGroupsPermissions.Add(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        // Atualizar vínculo (apenas status e datas)
        public async Task<bool> UpdateStatusAsync(Guid userId, Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
        {
            var entity = await _context.UsersGroupsPermissions
                .FirstOrDefaultAsync(ugp => ugp.UserId == userId && ugp.GroupId == groupId && ugp.PermissionId == permissionId, ct);

            if (entity == null) return false;

            entity.IsActive = isActive;
            entity.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return true;
        }

        // Excluir vínculo fisicamente
        public async Task<bool> DeleteAsync(Guid userId, Guid groupId, Guid permissionId, CancellationToken ct)
        {
            var entity = await _context.UsersGroupsPermissions
                .FirstOrDefaultAsync(ugp => ugp.UserId == userId && ugp.GroupId == groupId && ugp.PermissionId == permissionId, ct);

            if (entity == null) return false;

            _context.UsersGroupsPermissions.Remove(entity);
            await _context.SaveChangesAsync(ct);
            return true;
        }

        // Listar permissões de um usuário em um grupo
        public async Task<IEnumerable<UsersGroupsPermissions>> GetByUserGroupAsync(Guid userId, Guid groupId, CancellationToken ct)
            => await _context.UsersGroupsPermissions
                .Where(ugp => ugp.UserId == userId && ugp.GroupId == groupId)
                .ToListAsync(ct);

        // Listar todos os vínculos
        public async Task<IEnumerable<UsersGroupsPermissions>> GetAllAsync(CancellationToken ct)
            => await _context.UsersGroupsPermissions.ToListAsync(ct);

        public async Task UpdateAllUsersGroupsPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            var entities = await _context.UsersGroupsPermissions
                .Where(ugp => ugp.PermissionId == id)
                .ToListAsync(cancellationToken);

            foreach (var entity in entities)
            {
                entity.IsActive = isActive;
                entity.UpdatedAt = DateTime.UtcNow;
            }

            //await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
