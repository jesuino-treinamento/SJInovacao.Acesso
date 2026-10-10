using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Projections;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using System.Linq.Expressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupPermissionUserRepository
        : BaseRepository<UserGroup>, IGroupPermissionUserRepository
    {
        public GroupPermissionUserRepository(
            DefaultContext context,
            ILogger<GroupPermissionUserRepository> logger)
            : base(context, logger) { }

        // =========================
        // LEITURA
        // =========================

        public Task<UserGroup?> GetByIdAsync(Guid userId, Guid groupId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetUserGroupById:{userId}:{groupId}",
                async () => await _context.UserGroup
                    .AsNoTracking()
                    .FirstOrDefaultAsync(ug => ug.UserId == userId
                                             && ug.GroupId == groupId, ct));

        public Task<PaginatedList<UserGroupSummary>> GetAllPaginatedAsync(
            int page, int size, string? orderBy, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetAllUserGroupsPaginated:{page}:{size}",
                async () =>
                {
                    if (page < 1) page = 1;
                    if (size < 1) size = 10;
                    if (size > 100) size = 100;

                    // 1. Query base sobre a ENTIDADE (não a projeção)
                    var baseQuery = _context.UserGroup.AsNoTracking();

                    // 2. Count sobre a base SEM ordenação (mais rápido)
                    var total = await baseQuery.CountAsync(ct);

                    // 3. Ordena sobre a ENTIDADE (o EF sabe traduzir)
                    var orderedQuery = ApplyOrder(baseQuery, orderBy);

                    // 4. Pagina e PROJETA no final (Select depois de Skip/Take)
                    var items = await orderedQuery
                        .Skip((page - 1) * size)
                        .Take(size)
                        .Select(ug => new UserGroupSummary(
                            ug.UserId,
                            (ug.User.Name.FirstName ?? "") + " " + (ug.User.Name.LastName ?? ""),
                            ug.GroupId,
                            ug.Group.Name ?? "",
                            ug.IsActive))
                        .ToListAsync(ct);

                    return new PaginatedList<UserGroupSummary>(items, total, page, size);
                });

        public Task<List<UserGroupWithPermissionsSummary>> GetAllWithFiltersAsync(
            bool onlyActiveUsers,
            string? permissionNameFilter,
            int? maxResults,
            CancellationToken ct)
            => ExecuteWithLoggingAsync(
                "GetAllWithFilters",
                async () =>
                {
                    // ===== 1. Query principal (sem permissões) =====
                    var baseQuery = _context.UserGroup
                        .AsNoTracking()
                        .Where(ug => !onlyActiveUsers || ug.IsActive);

                    if (!string.IsNullOrWhiteSpace(permissionNameFilter))
                    {
                        var pattern = $"%{permissionNameFilter}%";
                        baseQuery = baseQuery.Where(ug => ug.Group.UsersGroupsPermissions
                            .Any(ugp => EF.Functions.ILike(ugp.Permission.Name, pattern)));
                    }

                    if (maxResults.HasValue)
                        baseQuery = baseQuery.Take(maxResults.Value);

                    var parents = await baseQuery
                        .Select(ug => new
                        {
                            ug.UserId,
                            UserName = (ug.User.Name.FirstName ?? "") + " " + (ug.User.Name.LastName ?? ""),
                            ug.GroupId,
                            GroupName = ug.Group.Name ?? "",
                            ug.IsActive
                        })
                        .ToListAsync(ct);

                    if (parents.Count == 0)
                        return new List<UserGroupWithPermissionsSummary>();

                    // ===== 2. Query das permissões (só dos IDs retornados) =====
                    var userIds = parents.Select(p => p.UserId).Distinct().ToList();
                    var groupIds = parents.Select(p => p.GroupId).Distinct().ToList();

                    var permissions = await _context.UsersGroupsPermissions
                        .AsNoTracking()
                        .Where(ugp => userIds.Contains(ugp.UserId)
                                   && groupIds.Contains(ugp.GroupId)
                                   && ugp.IsActive
                                   && ugp.Permission != null)
                        .Select(ugp => new
                        {
                            ugp.UserId,
                            ugp.GroupId,
                            Id = ugp.Permission!.Id,
                            Name = ugp.Permission!.Name,
                            Description = ugp.Permission!.Description,
                            IsActive = ugp.IsActive,
                            ugp.CreatedAt,
                            ugp.UpdatedAt
                        })
                        .ToListAsync(ct);

                    var permLookup = permissions
                        .GroupBy(p => (p.UserId, p.GroupId))
                        .ToDictionary(g => g.Key, g => g.ToList());

                    // ===== 3. Combina em memória =====
                    return parents.Select(p => new UserGroupWithPermissionsSummary(
                        p.UserId,
                        p.UserName,
                        p.GroupId,
                        p.GroupName,
                        p.IsActive,
                        permLookup.TryGetValue((p.UserId, p.GroupId), out var list)
                            ? list.Select(x => new GroupUserPermissionSummary(
                                x.Id, x.Name, x.Description, x.IsActive, x.CreatedAt, x.UpdatedAt))
                                  .ToList()
                            : new List<GroupUserPermissionSummary>()
                    )).ToList();
                });

        public Task<Dictionary<(Guid UserId, Guid GroupId), List<Permission>>> GetPermissionsForPageAsync(
            IReadOnlyList<Guid> userIds,
            IReadOnlyList<Guid> groupIds,
            CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetPermissionsForPage:{userIds.Count}:{groupIds.Count}",
                async () =>
                {
                    if (userIds.Count == 0 || groupIds.Count == 0)
                        return new Dictionary<(Guid, Guid), List<Permission>>();

                    var rows = await _context.UsersGroupsPermissions
                        .AsNoTracking()
                        .Where(ugp => userIds.Contains(ugp.UserId)
                                   && groupIds.Contains(ugp.GroupId)
                                   && ugp.IsActive
                                   && ugp.Permission != null)
                        .Select(ugp => new
                        {
                            ugp.UserId,
                            ugp.GroupId,
                            Permission = ugp.Permission!
                        })
                        .ToListAsync(ct);

                    return rows
                        .GroupBy(r => (r.UserId, r.GroupId))
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(x => x.Permission).ToList());
                });

        // =========================
        // ORDENAÇÃO — opera sobre a ENTIDADE
        // =========================

        private IQueryable<UserGroup> ApplyOrder(IQueryable<UserGroup> query, string? orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return query
                    .OrderBy(ug => ug.User.Name.FirstName)
                    .ThenBy(ug => ug.GroupId)
                    .ThenBy(ug => ug.UserId);

            var orderParams = orderBy
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            bool isFirst = true;

            foreach (var param in orderParams)
            {
                var parts = param.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                var field = parts[0];
                var direction = parts.Length > 1 ? parts[1] : "asc";

                query = ApplyOrderToField(query, field, direction, ref isFirst);
            }

            // Garante ordenação determinística
            if (query is IOrderedQueryable<UserGroup> ordered)
                return ordered.ThenBy(ug => ug.UserId).ThenBy(ug => ug.GroupId);

            return query
                .OrderBy(ug => ug.User.Name.FirstName)
                .ThenBy(ug => ug.UserId)
                .ThenBy(ug => ug.GroupId);
        }

        private IQueryable<UserGroup> ApplyOrderToField(
            IQueryable<UserGroup> query, string field, string direction, ref bool isFirst)
        {
            return field.ToLowerInvariant() switch
            {
                "username" => ApplyDirection(query, ug => ug.User.Name.FirstName, direction, ref isFirst),
                "userid" => ApplyDirection(query, ug => ug.UserId, direction, ref isFirst),
                "groupname" => ApplyDirection(query, ug => ug.Group.Name, direction, ref isFirst),
                "name" => ApplyDirection(query, ug => ug.Group.Name, direction, ref isFirst),
                "groupid" => ApplyDirection(query, ug => ug.GroupId, direction, ref isFirst),
                _ => query
            };
        }

        private IQueryable<UserGroup> ApplyDirection<TKey>(
            IQueryable<UserGroup> query,
            Expression<Func<UserGroup, TKey>> keySelector,
            string direction,
            ref bool isFirst)
        {
            var isDesc = direction.Equals("desc", StringComparison.OrdinalIgnoreCase);

            if (isFirst)
            {
                isFirst = false;
                return isDesc
                    ? query.OrderByDescending(keySelector)
                    : query.OrderBy(keySelector);
            }

            var ordered = (IOrderedQueryable<UserGroup>)query;

            return isDesc
                ? ordered.ThenByDescending(keySelector)
                : ordered.ThenBy(keySelector);
        }

        // =========================
        // ESCRITA
        // =========================

        public Task DeleteAsync(Guid userId, Guid groupId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"DeleteUserGroup:{userId}:{groupId}",
                async () =>
                {
                    var junction = await _context.UserGroup
                        .FirstOrDefaultAsync(ug => ug.UserId == userId
                                                 && ug.GroupId == groupId, ct);

                    if (junction is null)
                        return;

                    _context.UserGroup.Remove(junction);
                    await _context.SaveChangesAsync(ct);
                });
    }
}