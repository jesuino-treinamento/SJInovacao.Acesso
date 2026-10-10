using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UsersGroupsPermissionsRepository
        : BaseRepository<UsersGroupsPermissions>, IUsersGroupsPermissionsRepository
    {
        public UsersGroupsPermissionsRepository(
            DefaultContext context,
            ILogger<UsersGroupsPermissionsRepository> logger)
            : base(context, logger)
        {
        }

        public Task<UsersGroupsPermissions> CreateAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"CreateUGP:{userId}:{groupId}:{permissionId}",
                async () =>
                {
                    var exists = await _context.UsersGroupsPermissions
                        .AnyAsync(ugp => ugp.UserId == userId
                                      && ugp.GroupId == groupId
                                      && ugp.PermissionId == permissionId, ct);

                    if (exists)
                        throw new DomainException("Este vínculo já existe.");

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
                });

        public Task<bool> UpdateStatusAsync(
            Guid userId, Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"UpdateUGPStatus:{userId}:{groupId}:{permissionId}",
                async () =>
                {
                    var entity = await _context.UsersGroupsPermissions
                        .FirstOrDefaultAsync(ugp => ugp.UserId == userId
                                                 && ugp.GroupId == groupId
                                                 && ugp.PermissionId == permissionId, ct);

                    if (entity is null) return false;
                    if (entity.IsActive == isActive) return true;

                    entity.IsActive = isActive;
                    entity.UpdatedAt = DateTime.UtcNow;
                    await _context.SaveChangesAsync(ct);
                    return true;
                });

        public Task<bool> DeleteAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"DeleteUGP:{userId}:{groupId}:{permissionId}",
                async () =>
                {
                    var entity = await _context.UsersGroupsPermissions
                        .FirstOrDefaultAsync(ugp => ugp.UserId == userId
                                                 && ugp.GroupId == groupId
                                                 && ugp.PermissionId == permissionId, ct);

                    if (entity is null) return false;

                    _context.UsersGroupsPermissions.Remove(entity);
                    await _context.SaveChangesAsync(ct);
                    return true;
                });

        public Task UpdateAllByPermissionIdAsync(
            Guid permissionId, bool isActive, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"UpdateAllUGPByPermission:{permissionId}:{isActive}",
                async () =>
                {
                    var entities = await _context.UsersGroupsPermissions
                        .Where(ugp => ugp.PermissionId == permissionId)
                        .ToListAsync(ct);

                    foreach (var entity in entities)
                    {
                        entity.IsActive = isActive;
                        entity.UpdatedAt = DateTime.UtcNow;
                    }

                    await _context.SaveChangesAsync(ct);
                });

        public Task<IReadOnlyList<Permission>> UpdateUserGroupPermissionsAsync(
    Guid userId,
    Guid groupId,
    bool userIsActive,
    IEnumerable<Guid> permissionIds,
    bool? permissionIsActive,
    CancellationToken ct)
    => ExecuteWithLoggingAsync(
        $"UpdateUserGroupPermissions:{userId}:{groupId}",
        async () =>
        {
            var userGroupLinks = await _context.UsersGroupsPermissions
                .Where(ugp => ugp.UserId == userId && ugp.GroupId == groupId)
                .ToListAsync(ct);

            if (userGroupLinks.Count == 0)
                throw new DomainException(
                    $"Nenhum vínculo encontrado para o usuário {userId} no grupo {groupId}.");

            var permissionIdList = permissionIds?.Distinct().ToList() ?? new List<Guid>();

            if (permissionIdList.Count > 0 || permissionIsActive.HasValue)
            {
                var existentes = userGroupLinks
                    .Select(ugp => ugp.PermissionId)
                    .ToHashSet();

                var ausentes = permissionIdList
                    .Where(pid => !existentes.Contains(pid))
                    .ToList();

                if (ausentes.Count > 0)
                {
                    throw new DomainException(
                        $"As seguintes permissões não pertencem ao grupo {groupId} ou não existe(m): " +
                        $"[{string.Join(", ", ausentes)}]. Nada foi alterado.");
                }
            }

            var agora = DateTime.UtcNow;

            foreach (var link in userGroupLinks)
            {
                link.IsActive = userIsActive;
                link.UpdatedAt = agora;
            }

            if (permissionIdList.Count > 0 && permissionIsActive.HasValue)
            {
                foreach (var link in userGroupLinks
                             .Where(l => permissionIdList.Contains(l.PermissionId)))
                {
                    link.IsActive = userIsActive && permissionIsActive.Value;
                    link.UpdatedAt = agora;
                }
            }

            await _context.SaveChangesAsync(ct);

           var permissions = await _context.UsersGroupsPermissions
                .AsNoTracking()
                .Where(ugp => ugp.UserId == userId
                           && ugp.GroupId == groupId
                           && ugp.IsActive)
                .Select(ugp => ugp.Permission!)
                .Distinct()
                .ToListAsync(ct);

            return (IReadOnlyList<Permission>)permissions;
        });                

        public Task<UsersGroupsPermissions?> GetAsync(
            Guid userId, Guid groupId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetUGP:{userId}:{groupId}:{permissionId}",
                async () =>
                {
                    return await _context.UsersGroupsPermissions
                        .AsNoTracking()
                        .FirstOrDefaultAsync(ugp => ugp.UserId == userId
                                                 && ugp.GroupId == groupId
                                                 && ugp.PermissionId == permissionId, ct);
                });

        public Task<IReadOnlyList<UsersGroupsPermissions>> GetByUserGroupAsync(
            Guid userId, Guid groupId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetByUserGroup:{userId}:{groupId}",
                async () =>
                {
                    var list = await _context.UsersGroupsPermissions
                        .AsNoTracking()
                        .Include(ugp => ugp.Group)
                        .Include(ugp => ugp.Permission)
                        .Where(ugp => ugp.UserId == userId && ugp.GroupId == groupId)
                        .ToListAsync(ct);

                    return (IReadOnlyList<UsersGroupsPermissions>)list;
                });

        public Task<IReadOnlyList<UsersGroupsPermissions>> GetAllAsync(
            int skip, int take, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetAllUGP:{skip}:{take}",
                async () =>
                {
                    var list = await _context.UsersGroupsPermissions
                        .AsNoTracking()
                        .OrderBy(ugp => ugp.CreatedAt)
                        .Skip(skip)
                        .Take(take)
                        .ToListAsync(ct);

                    return (IReadOnlyList<UsersGroupsPermissions>)list;
                });

        public Task<PaginatedList<UsersGroupsPermissions>> GetAllPaginatedAsync(
            int page, int size, string orderBy, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetAllUGPPaginated:{page}:{size}",
                async () =>
                {
                    var query = _context.UsersGroupsPermissions.AsNoTracking();

                    query = orderBy?.ToLower() switch
                    {
                        "createdat" => query.OrderBy(ugp => ugp.CreatedAt),
                        "updatedat" => query.OrderBy(ugp => ugp.UpdatedAt),
                        _ => query.OrderBy(ugp => ugp.CreatedAt)
                    };

                    var total = await query.CountAsync(ct);

                    var items = await query
                        .Skip((page - 1) * size)
                        .Take(size)
                        .ToListAsync(ct);

                    return new PaginatedList<UsersGroupsPermissions>(
                        items, total, page, size);
                });
    }
}