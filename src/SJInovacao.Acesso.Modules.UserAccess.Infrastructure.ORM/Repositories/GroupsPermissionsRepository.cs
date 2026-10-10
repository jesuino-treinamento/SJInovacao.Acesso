//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

//namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
//{
//    public class GroupsPermissionsRepository
//        : BaseRepository<GroupsPermissions>, IGroupsPermissionsRepository
//    {
//        public GroupsPermissionsRepository(
//            DefaultContext context,
//            ILogger<GroupsPermissionsRepository> logger)
//            : base(context, logger) { }

//        // =========================
//        // LEITURA
//        // =========================

//        public Task<GroupsPermissions?> GetByIdAsync(
//            Guid groupId, Guid permissionId, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"GetById:{groupId}:{permissionId}",
//                async () => await _context.GroupsPermissions
//                    .AsNoTracking()
//                    .FirstOrDefaultAsync(gp => gp.GroupId == groupId
//                                            && gp.PermissionId == permissionId, ct));

//        public Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                "GetAll",
//                async () => await _context.GroupsPermissions
//                    .AsNoTracking()
//                    .ToListAsync(ct));

//        public Task<IEnumerable<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"GetByGroupId:{groupId}",
//                async () => await _context.GroupsPermissions
//                    .AsNoTracking()
//                    .Where(gp => gp.GroupId == groupId)
//                    .ToListAsync(ct));

//        public Task<IEnumerable<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"GetByPermissionId:{permissionId}",
//                async () => await _context.GroupsPermissions
//                    .AsNoTracking()
//                    .Where(gp => gp.PermissionId == permissionId)
//                    .ToListAsync(ct));

//        // =========================
//        // ESCRITA
//        // =========================

//        public Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"Create:{entity.GroupId}:{entity.PermissionId}",
//                async () =>
//                {
//                    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

//                    try
//                    {
//                        var groupExists = await _context.GroupPermissions
//                            .AsNoTracking()
//                            .AnyAsync(g => g.Id == entity.GroupId, ct);

//                        if (!groupExists)
//                            throw new DomainException($"Grupo {entity.GroupId} não encontrado.");

//                        var permissionExists = await _context.Permissions
//                            .AsNoTracking()
//                            .AnyAsync(p => p.Id == entity.PermissionId, ct);

//                        if (!permissionExists)
//                            throw new DomainException($"Permissão {entity.PermissionId} não encontrada.");

//                        // Não precisa setar navegações — apenas as FKs
//                        entity.Group = null!;
//                        entity.Permission = null!;

//                        await _context.GroupsPermissions.AddAsync(entity, ct);
//                        await _context.SaveChangesAsync(ct);
//                        await transaction.CommitAsync(ct);

//                        _logger.LogInformation(
//                            "Vínculo grupo-permissão criado. Grupo: {GroupId}, Permissão: {PermissionId}",
//                            entity.GroupId, entity.PermissionId);

//                        return entity;
//                    }
//                    catch (DomainException)
//                    {
//                        await transaction.RollbackAsync(ct);
//                        throw;
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogCritical(ex,
//                            "Erro inesperado ao criar vínculo grupo {GroupId} ↔ permissão {PermissionId}",
//                            entity.GroupId, entity.PermissionId);
//                        await transaction.RollbackAsync(ct);
//                        throw;
//                    }
//                });

//        public Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"Update:{entity.GroupId}:{entity.PermissionId}",
//                async () =>
//                {
//                    _context.GroupsPermissions.Update(entity);
//                    await _context.SaveChangesAsync(ct);

//                    _logger.LogInformation(
//                        "Vínculo grupo-permissão atualizado. Grupo: {GroupId}, Permissão: {PermissionId}",
//                        entity.GroupId, entity.PermissionId);

//                    return entity;
//                });

//        public Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"Delete:{groupId}:{permissionId}",
//                async () =>
//                {
//                    var entity = await _context.GroupsPermissions
//                        .FirstOrDefaultAsync(gp => gp.GroupId == groupId
//                                                && gp.PermissionId == permissionId, ct);

//                    if (entity is null)
//                    {
//                        _logger.LogWarning(
//                            "Vínculo não encontrado para exclusão. Grupo: {GroupId}, Permissão: {PermissionId}",
//                            groupId, permissionId);
//                        return false;
//                    }

//                    _context.GroupsPermissions.Remove(entity);
//                    await _context.SaveChangesAsync(ct);

//                    _logger.LogInformation(
//                        "Vínculo grupo-permissão removido. Grupo: {GroupId}, Permissão: {PermissionId}",
//                        groupId, permissionId);

//                    return true;
//                });

//        public Task UpdateStatusAsync(
//            Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"UpdateStatus:{groupId}:{permissionId}:{isActive}",
//                async () =>
//                {
//                    using var transaction = await _context.Database.BeginTransactionAsync(ct);

//                    try
//                    {
//                        var groupPermissions = await _context.GroupsPermissions
//                            .Where(gp => gp.GroupId == groupId && gp.PermissionId == permissionId)
//                            .ToListAsync(ct);

//                        if (groupPermissions.Count == 0)
//                        {
//                            _logger.LogWarning(
//                                "Vínculo não encontrado. Grupo: {GroupId}, Permissão: {PermissionId}",
//                                groupId, permissionId);
//                            throw new DomainException(
//                                $"Permissão {permissionId} não encontrada para o grupo {groupId}.");
//                        }

//                        var now = DateTime.UtcNow;

//                        foreach (var gp in groupPermissions)
//                        {
//                            gp.IsActive = isActive;
//                            gp.UpdatedAt = now;
//                        }

//                        // Propaga para UsersGroupsPermissions
//                        var usersGroupsPermissions = await _context.UsersGroupsPermissions
//                            .Where(ugp => ugp.GroupId == groupId && ugp.PermissionId == permissionId)
//                            .ToListAsync(ct);

//                        foreach (var ugp in usersGroupsPermissions)
//                        {
//                            ugp.IsActive = isActive;
//                            ugp.UpdatedAt = now;
//                        }

//                        // Propaga para UserPermissions
//                        var userPermissions = await _context.UserPermissions
//                            .Where(up => up.PermissionId == permissionId)
//                            .ToListAsync(ct);

//                        foreach (var up in userPermissions)
//                        {
//                            up.IsActive = isActive;
//                            up.UpdatedAt = now;
//                        }

//                        await _context.SaveChangesAsync(ct);
//                        await transaction.CommitAsync(ct);

//                        _logger.LogInformation(
//                            "Status atualizado com sucesso. Grupo: {GroupId}, Permissão: {PermissionId}, Ativo: {IsActive}",
//                            groupId, permissionId, isActive);
//                    }
//                    catch (DomainException)
//                    {
//                        await transaction.RollbackAsync(ct);
//                        throw;
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.LogCritical(ex,
//                            "Erro inesperado ao atualizar status. Grupo: {GroupId}, Permissão: {PermissionId}",
//                            groupId, permissionId);
//                        await transaction.RollbackAsync(ct);
//                        throw;
//                    }
//                });

//        public Task UpdateAllGroupsPermissionStatusAsync(
//            Guid permissionId, bool isActive, CancellationToken ct)
//            => ExecuteWithLoggingAsync(
//                $"UpdateAllByPermission:{permissionId}:{isActive}",
//                async () =>
//                {
//                    var groupsPermissions = await _context.GroupsPermissions
//                        .Where(gp => gp.PermissionId == permissionId)
//                        .ToListAsync(ct);

//                    if (groupsPermissions.Count == 0)
//                    {
//                        _logger.LogWarning(
//                            "Nenhum vínculo encontrado para a permissão {PermissionId}",
//                            permissionId);
//                        return;
//                    }

//                    var now = DateTime.UtcNow;

//                    foreach (var gp in groupsPermissions)
//                    {
//                        gp.IsActive = isActive;
//                        gp.UpdatedAt = now;
//                    }

//                    await _context.SaveChangesAsync(ct);

//                    _logger.LogInformation(
//                        "{Count} vínculos da permissão {PermissionId} atualizados para Ativo: {IsActive}",
//                        groupsPermissions.Count, permissionId, isActive);
//                });
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupsPermissionsRepository
        : BaseRepository<GroupsPermissions>, IGroupsPermissionsRepository
    {
        public GroupsPermissionsRepository(
            DefaultContext context,
            ILogger<GroupsPermissionsRepository> logger)
            : base(context, logger) { }

        // =========================
        // LEITURA
        // =========================

        public Task<GroupsPermissions?> GetByIdAsync(
            Guid groupId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetById:{groupId}:{permissionId}",
                async () => await _context.GroupsPermissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(gp => gp.GroupId == groupId
                                            && gp.PermissionId == permissionId, ct));

        public Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct)
            => ExecuteWithLoggingAsync(
                "GetAll",
                async () => await _context.GroupsPermissions
                    .AsNoTracking()
                    .ToListAsync(ct));

        public Task<List<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
             => ExecuteWithLoggingAsync(
                 $"GetByGroupId:{groupId}",
                 async () => await _context.GroupsPermissions
                     .AsNoTracking()
                     .Where(gp => gp.GroupId == groupId)
                     .ToListAsync(ct));

        public Task<List<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync<List<GroupsPermissions>>(
                $"GetByPermissionId:{permissionId}",
                async () => await _context.GroupsPermissions
                    .AsNoTracking()
                    .Where(gp => gp.PermissionId == permissionId)
                    .ToListAsync(ct));


        // =========================
        // ESCRITA
        // =========================

        public Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"Create:{entity.GroupId}:{entity.PermissionId}",
                async () =>
                {
                    await using var transaction = await _context.Database.BeginTransactionAsync(ct);

                    try
                    {
                        var groupExists = await _context.GroupPermissions
                            .AsNoTracking()
                            .AnyAsync(g => g.Id == entity.GroupId, ct);

                        if (!groupExists)
                            throw new DomainException($"Grupo {entity.GroupId} não encontrado.");

                        var permissionExists = await _context.Permissions
                            .AsNoTracking()
                            .AnyAsync(p => p.Id == entity.PermissionId, ct);

                        if (!permissionExists)
                            throw new DomainException($"Permissão {entity.PermissionId} não encontrada.");

                        // Não precisa setar navegações — apenas as FKs
                        entity.Group = null!;
                        entity.Permission = null!;

                        await _context.GroupsPermissions.AddAsync(entity, ct);
                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);

                        _logger.LogInformation(
                            "Vínculo grupo-permissão criado. Grupo: {GroupId}, Permissão: {PermissionId}",
                            entity.GroupId, entity.PermissionId);

                        return entity;
                    }
                    catch (DomainException)
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex,
                            "Erro inesperado ao criar vínculo grupo {GroupId} ↔ permissão {PermissionId}",
                            entity.GroupId, entity.PermissionId);
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });

        public Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"Update:{entity.GroupId}:{entity.PermissionId}",
                async () =>
                {
                    _context.GroupsPermissions.Update(entity);
                    await _context.SaveChangesAsync(ct);

                    _logger.LogInformation(
                        "Vínculo grupo-permissão atualizado. Grupo: {GroupId}, Permissão: {PermissionId}",
                        entity.GroupId, entity.PermissionId);

                    return entity;
                });

        public Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"Delete:{groupId}:{permissionId}",
                async () =>
                {
                    var entity = await _context.GroupsPermissions
                        .FirstOrDefaultAsync(gp => gp.GroupId == groupId
                                                && gp.PermissionId == permissionId, ct);

                    if (entity is null)
                    {
                        _logger.LogWarning(
                            "Vínculo não encontrado para exclusão. Grupo: {GroupId}, Permissão: {PermissionId}",
                            groupId, permissionId);
                        return false;
                    }

                    _context.GroupsPermissions.Remove(entity);
                    await _context.SaveChangesAsync(ct);

                    _logger.LogInformation(
                        "Vínculo grupo-permissão removido. Grupo: {GroupId}, Permissão: {PermissionId}",
                        groupId, permissionId);

                    return true;
                });

        public Task UpdateStatusAsync(
            Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"UpdateStatus:{groupId}:{permissionId}:{isActive}",
                async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync(ct);

                    try
                    {
                        var groupPermissions = await _context.GroupsPermissions
                            .Where(gp => gp.GroupId == groupId && gp.PermissionId == permissionId)
                            .ToListAsync(ct);

                        if (groupPermissions.Count == 0)
                        {
                            _logger.LogWarning(
                                "Vínculo não encontrado. Grupo: {GroupId}, Permissão: {PermissionId}",
                                groupId, permissionId);
                            throw new DomainException(
                                $"Permissão {permissionId} não encontrada para o grupo {groupId}.");
                        }

                        var now = DateTime.UtcNow;

                        foreach (var gp in groupPermissions)
                        {
                            gp.IsActive = isActive;
                            gp.UpdatedAt = now;
                        }

                        // Propaga para UsersGroupsPermissions
                        var usersGroupsPermissions = await _context.UsersGroupsPermissions
                            .Where(ugp => ugp.GroupId == groupId && ugp.PermissionId == permissionId)
                            .ToListAsync(ct);

                        foreach (var ugp in usersGroupsPermissions)
                        {
                            ugp.IsActive = isActive;
                            ugp.UpdatedAt = now;
                        }

                        // Propaga para UserPermissions
                        var userPermissions = await _context.UserPermissions
                            .Where(up => up.PermissionId == permissionId)
                            .ToListAsync(ct);

                        foreach (var up in userPermissions)
                        {
                            up.IsActive = isActive;
                            up.UpdatedAt = now;
                        }

                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);

                        _logger.LogInformation(
                            "Status atualizado com sucesso. Grupo: {GroupId}, Permissão: {PermissionId}, Ativo: {IsActive}",
                            groupId, permissionId, isActive);
                    }
                    catch (DomainException)
                    {
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex,
                            "Erro inesperado ao atualizar status. Grupo: {GroupId}, Permissão: {PermissionId}",
                            groupId, permissionId);
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });

        public Task UpdateAllGroupsPermissionStatusAsync(
            Guid permissionId, bool isActive, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"UpdateAllByPermission:{permissionId}:{isActive}",
                async () =>
                {
                    var groupsPermissions = await _context.GroupsPermissions
                        .Where(gp => gp.PermissionId == permissionId)
                        .ToListAsync(ct);

                    if (groupsPermissions.Count == 0)
                    {
                        _logger.LogWarning(
                            "Nenhum vínculo encontrado para a permissão {PermissionId}",
                            permissionId);
                        return;
                    }

                    var now = DateTime.UtcNow;

                    foreach (var gp in groupsPermissions)
                    {
                        gp.IsActive = isActive;
                        gp.UpdatedAt = now;
                    }

                    await _context.SaveChangesAsync(ct);

                    _logger.LogInformation(
                        "{Count} vínculos da permissão {PermissionId} atualizados para Ativo: {IsActive}",
                        groupsPermissions.Count, permissionId, isActive);
                });
    }
}