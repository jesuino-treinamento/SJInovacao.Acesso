using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupPermissionRepository : BaseRepository<GroupPermission>, IGroupPermissionRepository
    {
        private readonly DefaultContext _context;
        private readonly ILogger<GroupPermissionRepository> _logger;

        public GroupPermissionRepository(DefaultContext context, ILogger<GroupPermissionRepository> logger)
        : base(context, logger)
        {
            _context = context; _logger = logger;
        }

        public async Task<GroupPermission?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await ExecuteWithLoggingAsync(
            "GetByIdGroupPermissions",
            async () =>
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
            });
        }       

        public async Task<GroupPermission> CreateAsync(GroupPermission groupPermission, CancellationToken ct)
        {
            return await ExecuteWithLoggingAsync(
            "CreateGroupPermissions",
            async () =>
            {
                using var transaction = await _context.Database.BeginTransactionAsync(ct);
                try
                {
                    _logger.LogInformation("Iniciando criação do grupo {GroupName}", groupPermission.Name);

                    if (await GroupNameExistsAsync(groupPermission.Name, ct))
                    {
                        _logger.LogWarning("Grupo {GroupName} já existe", groupPermission.Name);
                        throw new DomainException($"Grupo {groupPermission.Name} já existe!");
                    }
                    var group = new GroupPermission(groupPermission.Name, groupPermission.Description);
                    await _context.GroupPermissions.AddAsync(group, ct);
                    await _context.SaveChangesAsync(ct);
                    await _context.GroupsPermissions.AddRangeAsync(groupPermission.Permissions.Select(permission => new GroupsPermissions
                    {
                        GroupId = group.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    }), ct);
                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    _logger.LogInformation("Grupo {GroupName} criado com sucesso", groupPermission.Name);

                    group.Permissions = groupPermission.Permissions;
                    return group;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Erro inesperado ao criar grupo {GroupName}", groupPermission.Name);
                    await transaction.RollbackAsync(ct);
                    throw;
                }
            });
        }

        public async Task<GroupPermission> UpdateAsync(GroupPermission groupPermission, CancellationToken ct)
        {
            using var transaction = await Context.Database.BeginTransactionAsync(ct);           
            return await ExecuteWithLoggingAsync(
            "UpdateGroupPermissions",
            async () =>
            {
                try
                {
                    var exists = await Context.GroupPermissions
                    .AnyAsync(gp => gp.Name == groupPermission.Name && gp.Id != groupPermission.Id, ct);

                    if (exists)
                        throw new DomainException($"Já existe um grupo com o nome '{groupPermission.Name}'.");

                    var existingLinks = await Context.GroupsPermissions
                        .Where(gp => gp.GroupId == groupPermission.Id)
                        .ToListAsync(ct);

                    foreach (var gp in existingLinks.Where(gp => !groupPermission.GroupsPermissions.Any(gp2 => gp2.PermissionId == gp.PermissionId)))
                    {
                        Context.GroupsPermissions.Remove(gp);

                        Context.UsersGroupsPermissions.RemoveRange(
                            Context.UsersGroupsPermissions.Where(x => x.GroupId == groupPermission.Id && x.PermissionId == gp.PermissionId)
                        );

                        Context.UserPermissions.RemoveRange(
                            Context.UserPermissions.Where(x => x.UserId == groupPermission.Id && x.PermissionId == gp.PermissionId)
                        );
                    }

                    var permissionIds = groupPermission.GroupsPermissions
                        .Select(gp => gp.PermissionId)
                        .ToList();

                    var groupPermissionsToUpdate = await Context.GroupsPermissions
                        .Where(gp => gp.GroupId == groupPermission.Id && permissionIds.Contains(gp.PermissionId))
                        .ToListAsync(ct);

                    foreach (var gp in groupPermissionsToUpdate)
                    {
                        gp.IsActive = groupPermission.IsActive;
                        gp.UpdatedAt = DateTime.UtcNow;
                    }
                    Context.GroupsPermissions.UpdateRange(groupPermissionsToUpdate);

                    var entities = await Context.UsersGroupsPermissions
                        .Where(ugp => ugp.GroupId == groupPermission.Id)
                        .ToListAsync(ct);

                    foreach (var entity in entities)
                    {
                        entity.IsActive = groupPermission.IsActive;
                        entity.UpdatedAt = DateTime.UtcNow;
                    }

                    var usersPermissions = await Context.UserPermissions
                        .Where(up => up.UserId == groupPermission.Id)
                        .ToListAsync(ct);

                    foreach (var userPermission in usersPermissions)
                    {
                        userPermission.IsActive = groupPermission.IsActive;
                        userPermission.UpdatedAt = DateTime.UtcNow;
                    }

                    groupPermission.UpdatedAt = DateTime.UtcNow;

                    var updatedGroup = Context.GroupPermissions.Update(groupPermission);

                    await Context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    Logger.LogInformation("Grupo {GroupId} atualizado com sucesso. Status: {Status}",
                        groupPermission.Id, groupPermission.IsActive ? "Ativo" : "Inativo");

                    return updatedGroup.Entity;
                }                    
                catch (DomainException ex)
                {
                    Logger.LogError(ex, "Erro de domínio ao atualizar grupo {GroupId}", groupPermission.Id);
                    await transaction.RollbackAsync(ct);
                    throw;
                }
                catch (Exception ex)
                {
                    Logger.LogCritical(ex, "Erro inesperado ao atualizar grupo {GroupId}", groupPermission.Id);
                    await transaction.RollbackAsync(ct);
                    throw;
                }
                    
            });
        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
        {
            using var transaction = await Context.Database.BeginTransactionAsync(ct);           
            return await ExecuteWithLoggingAsync(
            "DeleteGroupPermissions",
            async () =>
            {
                try
                {
                    var group = await GetByIdAsync(id, ct);
                    if (group == null) return false;

                    // Remove vínculos primeiro
                    var groupPermissions = await _context.GroupsPermissions
                        .Where(gp => gp.GroupId == id)
                        .ToListAsync(ct);
                    _context.GroupsPermissions.RemoveRange(groupPermissions);

                    var userGroupPermissions = await _context.UsersGroupsPermissions
                        .Where(ugp => ugp.GroupId == id)
                        .ToListAsync(ct);
                    _context.UsersGroupsPermissions.RemoveRange(userGroupPermissions);

                    var userGroup = await _context.UserGroup
                        .Where(ug => ug.GroupId == id)
                        .ToListAsync(ct);
                    _context.UserGroup.RemoveRange(userGroup);

                    _context.GroupPermissions.Remove(group);

                    await _context.SaveChangesAsync(ct);
                    await transaction.CommitAsync(ct);

                    return true;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(ct);
                    Logger.LogError(ex, "Erro ao excluir grupo {GroupId}", id);
                    throw;
                }
            });
        }


        public async Task<bool> GroupNameExistsAsync(string name, CancellationToken ct)
            => await _context.GroupPermissions.AnyAsync(g => g.Name == name, ct);

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
    }

}
