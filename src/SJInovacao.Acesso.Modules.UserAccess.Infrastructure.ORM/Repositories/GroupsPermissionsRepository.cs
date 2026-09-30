using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using System.Text.RegularExpressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    //public class GroupsPermissionsRepository : IGroupsPermissionsRepository
    //{
    //    private readonly DefaultContext _context;

    //    public GroupsPermissionsRepository(DefaultContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<GroupsPermissions?> GetByIdAsync(Guid groupId, Guid permissionId, CancellationToken ct)
    //    {
    //        return await _context.GroupsPermissions
    //            .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);
    //    }

    //    public async Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct)
    //    {
    //        return await _context.GroupsPermissions.ToListAsync(ct);
    //    }

    //    public async Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct)
    //    {
    //        _context.GroupsPermissions.Add(entity);
    //        await _context.SaveChangesAsync(ct);
    //        return entity;
    //    }

    //    public async Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct)
    //    {
    //        _context.GroupsPermissions.Update(entity);
    //        await _context.SaveChangesAsync(ct);
    //        return entity;
    //    }

    //    public async Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
    //    {
    //        var entity = await GetByIdAsync(groupId, permissionId, ct);
    //        if (entity == null) return false;

    //        _context.GroupsPermissions.Remove(entity);
    //        await _context.SaveChangesAsync(ct);
    //        return true;
    //    }

    //    public async Task<IEnumerable<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
    //    {
    //        return await _context.GroupsPermissions
    //            .Where(gp => gp.GroupId == groupId)
    //            .ToListAsync(ct);
    //    }

    //    public async Task<IEnumerable<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
    //    {
    //        return await _context.GroupsPermissions
    //            .Where(gp => gp.PermissionId == permissionId)
    //            .ToListAsync(ct);
    //    }

    //    public async Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
    //    {
    //        var entity = await GetByIdAsync(groupId, permissionId, ct);
    //        if (entity != null)
    //        {
    //            entity.IsActive = isActive;
    //            entity.UpdatedAt = DateTime.UtcNow;
    //            await _context.SaveChangesAsync(ct);
    //        }
    //    }
    //}

    public class GroupsPermissionsRepository : IGroupsPermissionsRepository
    {
        private readonly DefaultContext _context;

        public GroupsPermissionsRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<GroupsPermissions?> GetByIdAsync(Guid groupId, Guid permissionId, CancellationToken ct)
            => await _context.GroupsPermissions.FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);

        public async Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct)
            => await _context.GroupsPermissions.ToListAsync(ct);

        public async Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                // 1️⃣ Verifica se o grupo e a permissão existem no banco
                var groupExists = await _context.GroupPermissions
                    .AsNoTracking()
                    .AnyAsync(g => g.Id == entity.GroupId, ct);

                var permissionExists = await _context.Permissions
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == entity.PermissionId, ct);

                if (!groupExists)
                    throw new InvalidOperationException($"Grupo {entity.GroupId} não encontrado.");
                if (!permissionExists)
                    throw new InvalidOperationException($"Permissão {entity.PermissionId} não encontrada.");

                entity.Group = null;
                entity.Permission = null;
                // 2️⃣ Marca as entidades como não modificadas (sem recriar)
                //_context.Entry(entity.Group).State = EntityState.Unchanged;
                //_context.Entry(entity.Permission).State = EntityState.Unchanged;

                // 3️⃣ Adiciona o vínculo
                await _context.GroupsPermissions.AddAsync(entity, ct);

                // 4️⃣ Salva alterações
                await _context.SaveChangesAsync(ct);

                // 5️⃣ Commit da transação
                await transaction.CommitAsync(ct);

                return entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                throw new Exception($"Erro ao criar vínculo grupo-permissão: {ex.Message}", ex);
            }
        }



        public async Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct)
        {
            _context.GroupsPermissions.Update(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            // 1️⃣ Buscar o vínculo específico
            var entity = await _context.GroupsPermissions
                .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);

            if (entity == null)
                return false;

            // 2️⃣ Remover apenas o vínculo
            _context.GroupsPermissions.Remove(entity);

            // 3️⃣ Salvar alterações
            await _context.SaveChangesAsync(ct);

            return true;
        }


        public async Task<IEnumerable<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
            => await _context.GroupsPermissions.Where(gp => gp.GroupId == groupId).ToListAsync(ct);

        public async Task<IEnumerable<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
            => await _context.GroupsPermissions.Where(gp => gp.PermissionId == permissionId).ToListAsync(ct);

        public async Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
        {
            var entity = await GetByIdAsync(groupId, permissionId, ct);
            if (entity != null)
            {
                entity.IsActive = isActive;
                entity.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task UpdateAllGroupsPermissionStatusAsync(Guid permissionId, bool isActive, CancellationToken cancellationToken)
        {
            var groupsPermissions = _context.GroupsPermissions.Where(gp => gp.PermissionId == permissionId);
            await groupsPermissions.ForEachAsync(gp =>
            {
                gp.IsActive = isActive;
                gp.UpdatedAt = DateTime.UtcNow;
            }, cancellationToken);
           // await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
