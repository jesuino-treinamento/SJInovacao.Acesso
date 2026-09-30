using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using System.Text.RegularExpressions;
using System.Threading;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    //public class GroupPermissionRepository : IGroupPermissionRepository
    //{
    //    private readonly DefaultContext _context;

    //    public GroupPermissionRepository(DefaultContext context)
    //    {
    //        _context = context;
    //    }

    //    public async Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    //    {
    //        return await _context.GroupPermissions
    //            .Include(g => g.Users)
    //            .Include(g => g.Permissions)
    //            .FirstOrDefaultAsync(g => g.Id == id, cancellationToken);
    //    }

    //    public async Task<List<GroupPermission>> GetAllAsync(CancellationToken cancellationToken)
    //    {
    //        return await _context.GroupPermissions
    //            .Include(g => g.Users)
    //            .Include(g => g.Permissions)
    //            .ToListAsync(cancellationToken);
    //    }

    //    public async Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken)
    //    {
    //        return await _context.Permissions
    //            .Where(p => ids.Contains(p.Id) && p.IsActive)
    //            .ToListAsync(cancellationToken);
    //    }


    //    public async Task<GroupPermission> CreateAsync(GroupPermission groupPermission, CancellationToken cancellationToken)
    //    {
    //        ////Erro: An error occurred while saving the entity changes. See the inner exception for details.

    //        ////Opção 1 com erro
    //        //var exists = await _context.GroupPermissions
    //        //.AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);

    //        //if (exists)
    //        //    throw new DomainException("A groupUser with the same name already exists");

    //        //await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
    //        //await _context.SaveChangesAsync(cancellationToken);
    //        //return groupPermission;


    //        ////Opção 2 com erro
    //        //var exists = await _context.GroupPermissions
    //        //    .AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);
    //        //if (exists)
    //        //    throw new DomainException("A group with the same name already exists");

    //        //// Anexa permissões existentes (já devem estar no contexto ou serão anexadas)
    //        //foreach (var permission in groupPermission.Permissions)
    //        //{
    //        //    if (_context.Entry(permission).State == EntityState.Detached)
    //        //        _context.Entry(permission).State = EntityState.Unchanged;
    //        //}

    //        //await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
    //        //await _context.SaveChangesAsync(cancellationToken);
    //        //return groupPermission;

    //        try
    //        {
    //            var exists = await _context.GroupPermissions
    //                .AnyAsync(u => u.Name == groupPermission.Name, cancellationToken);
    //            if (exists)
    //                throw new DomainException("A group with the same name already exists");

    //            foreach (var permission in groupPermission.Permissions)
    //            {
    //                if (_context.Entry(permission).State == EntityState.Detached)
    //                    _context.Entry(permission).State = EntityState.Unchanged;
    //            }

    //            await _context.GroupPermissions.AddAsync(groupPermission, cancellationToken);
    //            await _context.SaveChangesAsync(cancellationToken);
    //            return groupPermission;
    //        }
    //        catch (DbUpdateException ex)
    //        {
    //            // Isso vai mostrar o erro real do banco
    //            var innerMessage = ex.InnerException?.Message;
    //            throw new Exception($"Erro no banco: {innerMessage}", ex);
    //        }
    //    }

    //    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    //    {
    //        //_context.GroupUsers.Remove(groupuser);
    //        //await _context.SaveChangesAsync(cancellationToken);

    //        var groupPermission = await GetByIdAsync(id, cancellationToken);
    //        if (groupPermission == null)
    //            return false;

    //        _context.GroupPermissions.Remove(groupPermission);
    //        await _context.SaveChangesAsync(cancellationToken);
    //        return true;
    //    }

    //    public void Remove(GroupPermission groupPermission)
    //    {
    //        _context.GroupPermissions.Remove(groupPermission);
    //    }

    //    public async Task<GroupPermission> UpdateAsync(GroupPermission groupPermission, CancellationToken cancellationToken = default)
    //    {
    //        _context.GroupPermissions.Update(groupPermission);
    //        await _context.SaveChangesAsync(cancellationToken);

    //        return groupPermission;
    //    }

    //    public async Task<bool> GroupNameExistsAsync(string name, Guid excludeGroupId, CancellationToken cancellationToken)
    //    {
    //        // Verifica se o novo nome já existe (excluindo o próprio grupo)
    //        var lowerName = name.ToLowerInvariant();
    //        return await _context.GroupPermissions
    //            .AnyAsync(g => g.Name.ToLower() == lowerName && g.Id != excludeGroupId, cancellationToken);
    //    }
    //    //Analisar entidade e tabela
    //    public async Task<IEnumerable<GroupPermission>> GetAllWithGroupPermissionsAsync(CancellationToken ct)
    //    {
    //        return await _context.GroupPermissions
    //             .Include(g => g.Permissions)
    //             .Include(g => g.UserGroups) // se quiser trazer também os usuários vinculados
    //             .ToListAsync(ct);
    //    }

    //    public async Task<IEnumerable<Permission>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
    //    {
    //        var group = await _context.GroupPermissions
    //            .Include(g => g.Permissions)
    //            .FirstOrDefaultAsync(g => g.Id == groupId, ct);

    //        if (group == null)
    //            return Enumerable.Empty<Permission>();

    //        return group.Permissions.Where(p => p.IsActive).ToList();
    //    }

    //    // Analisar melhor os classes e a tabela
    //    public async Task RemoveAsync(Guid groupId, Guid permissionId, CancellationToken ct)
    //    {
    //        var group = await _context.GroupPermissions
    //            .Include(g => g.Permissions)
    //            .FirstOrDefaultAsync(g => g.Id == groupId, ct);

    //        if (group == null)
    //            throw new DomainException("Group not found");

    //        var permission = group.Permissions.FirstOrDefault(p => p.Id == permissionId);
    //        if (permission == null)
    //            throw new DomainException("Permission not found in group");

    //        group.Permissions.Remove(permission);
    //        await _context.SaveChangesAsync(ct);
    //    }

    //    // Analisar melhor os classes e a tabela
    //    public async Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
    //    {
    //        var group = await _context.GroupPermissions
    //            .Include(g => g.Permissions)
    //            .FirstOrDefaultAsync(g => g.Id == groupId, ct);

    //        if (group == null)
    //            throw new DomainException("Group not found");

    //        var permission = group.Permissions.FirstOrDefault(p => p.Id == permissionId);
    //        if (permission == null)
    //            throw new DomainException("Permission not found in group");

    //        permission.IsActive = isActive;
    //        _context.Permissions.Update(permission);
    //        await _context.SaveChangesAsync(ct);
    //    }

    //}

    public class GroupPermissionRepository : IGroupPermissionRepository
    {
        private readonly DefaultContext _context;

        public GroupPermissionRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            var groupPermission = await _context.GroupPermissions
                .Include(p => p.UserGroups)
                    .ThenInclude(ug => ug.User)
                .Include(p => p.Permissions)
                    .ThenInclude(p => p.GroupsPermissions)
                .Include(p => p.GroupsPermissions)
                    .ThenInclude(gp => gp.Permission)
                .FirstOrDefaultAsync(p => p.Id == id, ct);
            
            return groupPermission;
        }
            //=> await _context.GroupPermissions
            //.Include(p => p.GroupsPermissions)
            //    .ThenInclude(gp => gp.Permission)
            //.Where(p => p.Id == id)
            //.FirstOrDefaultAsync(ct);

        public async Task<List<GroupPermission>> GetAllAsync(CancellationToken ct)
            => await _context.GroupPermissions.ToListAsync(ct);

        public async Task<GroupPermission> CreateAsync(GroupPermission entity, CancellationToken ct)
        {
            var exists = await _context.GroupPermissions
                .AnyAsync(u => u.Name == entity.Name, ct);
            if (exists)
                throw new DomainException("A group with the same name already exists");

            // Inclui apenas o grupo
            await _context.GroupPermissions.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);

            // Cria vínculos em GroupsPermissions (GroupId + PermissionId)
            foreach (var permission in entity.Permissions)
            {
                var gp = new GroupsPermissions
                {
                    GroupId = entity.Id,          // agora já existe o Id do grupo
                    PermissionId = permission.Id, // usa o Id da permissão existente
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                // Marca as permissões como "não modificadas" para evitar inserção indevida
                _context.Entry(permission).State = EntityState.Unchanged;

                _context.GroupsPermissions.Add(gp);
            }

            await _context.SaveChangesAsync(ct);
            return entity;
        }

        //public async Task<GroupPermission> CreateAsync(GroupPermission entity, CancellationToken ct)
        //{

        //    var exists = await _context.GroupPermissions
        //           .AnyAsync(u => u.Name == entity.Name, ct);
        //    if (exists)
        //        throw new DomainException("A group with the same name already exists");

        //    //foreach (var permission in entity.Permissions)
        //    //{
        //    //    if (_context.Entry(permission).State == EntityState.Detached)
        //    //        _context.Entry(permission).State = EntityState.Unchanged;
        //    //}

        //    await _context.GroupPermissions.AddAsync(entity, ct);

        //    //await _context.SaveChangesAsync(ct);

        //    // também cria vínculos em GroupsPermissions
        //    foreach (var permission in entity.Permissions)
        //    {
        //        var gp = new GroupsPermissions
        //        {
        //            //GroupId = entity.Id,
        //            //PermissionId = permission.Id,
        //            IsActive = true,
        //            CreatedAt = DateTime.UtcNow
        //        };
        //        _context.GroupsPermissions.Add(gp);
        //    }

        //    await _context.SaveChangesAsync(ct);
        //    return entity;
        //}

        public async Task<GroupPermission> UpdateAsync(GroupPermission entity, CancellationToken ct)
        {
            _context.GroupPermissions.Update(entity);

            // atualiza vínculos em GroupsPermissions
            var group = _context.GroupsPermissions.Where(gp => gp.GroupId == entity.Id);

            if (group == null)
                throw new InvalidOperationException("Grupo não encontrado.");           

            // 3️⃣ Atualizar o status dos vínculos em GroupsPermissions
            var groupPermissions = await _context.GroupsPermissions
                .Where(gp => gp.GroupId == entity.Id)
                .ToListAsync(ct);

            foreach (var gp in groupPermissions)
            {
                gp.IsActive = entity.IsActive;
                gp.UpdatedAt = DateTime.UtcNow;
            }

            // 4️⃣ Salvar alterações
            await _context.SaveChangesAsync(ct);

            return entity;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var group = await GetByIdAsync(id, ct);
            if (group == null) return false;

            //var groups = _context.GroupsPermissions.Where(gp => gp.GroupId == id).ToList();

            group.GroupsPermissions = null;


            _context.GroupPermissions.Remove(group);

            // também remove vínculos em GroupsPermissions
            var groupPermissions = _context.GroupsPermissions.Where(gp => gp.GroupId == id).ToList();
            _context.GroupsPermissions.RemoveRange(groupPermissions);

            await _context.SaveChangesAsync(ct);
            return true;
        }

        public async Task<bool> GroupNameExistsAsync(string name, Guid id, CancellationToken ct)
            => await _context.GroupPermissions.AnyAsync(g => g.Name == name && g.Id != id, ct);

        public async Task<List<Permission>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken ct)
            => await _context.Permissions.Where(p => ids.Contains(p.Id)).ToListAsync(ct);

        public async Task<IEnumerable<GroupPermission>> GetAllWithGroupPermissionsAsync(CancellationToken ct)
            => await _context.GroupPermissions.Include(p => p.GroupsPermissions).ThenInclude(g => g.Permission).ToListAsync(ct);

        public async Task<IEnumerable<Permission>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
            => await _context.GroupsPermissions
                .Where(gp => gp.GroupId == groupId)
                .Select(gp => gp.Permission)
                .ToListAsync(ct);

        public async Task RemoveAsync(Guid groupId, CancellationToken ct)
        {
            var entities = await _context.GroupsPermissions
                .Where(gp => gp.GroupId == groupId)
                .ToListAsync(ct);
            if (entities.Any())
            {
                _context.GroupsPermissions.RemoveRange(entities);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
        {
            var entity = await _context.GroupsPermissions
                .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);
            if (entity != null)
            {
                entity.IsActive = isActive;
                await _context.SaveChangesAsync(ct);
            }
        }
    }

}
