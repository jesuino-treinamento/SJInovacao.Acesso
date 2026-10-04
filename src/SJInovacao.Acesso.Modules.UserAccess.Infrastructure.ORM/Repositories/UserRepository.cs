
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
    
namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DefaultContext context, ILogger<UserRepository> logger)
        : base(context, logger)
        {
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await ExecuteWithLoggingAsync(
                "GetById",
                async () =>
                {
                    return await Context.Users
                        .AsNoTracking()
                        .FirstOrDefaultAsync(u => u.Id == id, ct);
                });
        }
        public async Task<User?> GetByIdUserAddressesPhonesAsync(Guid id, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
            "GetByIdUserAddressesPhones",
            async () =>
            {
                return await Context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Phones)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            
            });
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
            "Create",
            async () =>
            {
                Context.Users.Add(user);
                await Context.SaveChangesAsync(cancellationToken);
                return user;
            });
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            Context.Users.Update(user);
            await Context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default)
            => await Context.Users.AnyAsync(u => u.Email == email || u.Username == username, cancellationToken);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
            => await Context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
            => await Context.Users.FirstOrDefaultAsync(u => u.Username == name, cancellationToken);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user == null) return false;
            Context.Users.Remove(user);
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user == null) return false;
            user.Deactivate();
            await Context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy)
        {
            var query = Context.Users.AsQueryable();
            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (users, totalCount);
        }
        //Paginação de todos os usuários 
        public async Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy)
        {
            return await ExecuteWithLoggingAsync(
            "GetAllPaginated",
            async () =>
            {
                var query = Context.Users.AsQueryable();
                var totalCount = await query.CountAsync();
                var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
                return new PaginatedList<User>(items, totalCount, page, size);
            });
        }

        public IQueryable<User> Query() => Context.Users.AsQueryable();

        public async Task<User?> GetGroupToUserAsync(Guid userId, CancellationToken cancellationToken)
            => await Context.Users  
            .Include(u => u.UserPermissions)
                .ThenInclude(g => g.Permission)
            .Include(u => u.UserGroups)
                .ThenInclude(g => g.Group.Permissions)
            .Include(u => u.UsersGroupsPermissions)
                .ThenInclude(g => g.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        public async Task<User> RemoveGroupFromUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            var user = await GetGroupToUserAsync(userId, cancellationToken);

            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
                var group = user.UserGroups.FirstOrDefault(g =>g.UserId == userId && g.GroupId == groupId);

            if (user.UsersGroupsPermissions.Any())
                Context.UsersGroupsPermissions.RemoveRange(user.UsersGroupsPermissions.Where(ugp => ugp.GroupId == groupId));

            if (group != null) user.UserGroups.Remove(group);
                await Context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<List<Permission>> UpdateGroupPermissionAsync(
        Guid userId,
        Guid groupId,
        bool userIsActive,
        List<Guid>? permissionIds,
        bool? permissionIsActive,
        CancellationToken cancellationToken = default)
        {
            var user = await GetGroupToUserAsync(userId, cancellationToken);
            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");

            var group = user.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
            if (group == null) throw new InvalidOperationException("Grupo não encontrado.");

            // Atualiza status do grupo
            group.IsActive = userIsActive;

            var updatedPermissions = new List<Permission>();

            // Se não foram passados IDs de permissões, atualiza todas
            if (permissionIds == null || !permissionIds.Any())
            {
                foreach (var ugp in group.User.UsersGroupsPermissions
                    .Where(p => p.GroupId == groupId && p.UserId == userId))
                {
                    ugp.IsActive = userIsActive;
                    ugp.UpdatedAt = DateTime.UtcNow;
                    updatedPermissions.Add(ugp.Permission);
                }

                Context.UsersGroupsPermissions.UpdateRange(group.User.UsersGroupsPermissions);
            }
            else
            {
                foreach (var ugp in group.User.UsersGroupsPermissions
                    .Where(p => p.GroupId == groupId && p.UserId == userId && permissionIds.Contains(p.PermissionId)))
                {
                    ugp.IsActive = permissionIsActive ?? true;
                    ugp.UpdatedAt = DateTime.UtcNow;
                    updatedPermissions.Add(ugp.Permission);
                }

                Context.UsersGroupsPermissions.UpdateRange(group.User.UsersGroupsPermissions);
            }

            await Context.SaveChangesAsync(cancellationToken);

            // Retorna lista de permissões atualizadas
            return updatedPermissions;
        }

        public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            try
            {
                // 1️⃣ Carregar apenas o usuário e vínculos já existentes (sem rastrear tudo)
                var user = await Context.Users
                    .Include(u => u.UserGroups)
                    .Include(u => u.UsersGroupsPermissions)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                if (user == null)
                {
                    Logger.LogWarning("Usuário não encontrado. UserId={UserId}", userId);
                    throw new InvalidOperationException("Usuário não encontrado.");
                }

                // 2️⃣ Carregar apenas permissões ativas do grupo (projeção direta)
                var activePermissions = await Context.GroupsPermissions
                    .Where(gp => gp.GroupId == groupId && gp.IsActive)
                    .Select(gp => gp.PermissionId)
                    .ToListAsync(cancellationToken);

                if (!activePermissions.Any())
                {
                    Logger.LogWarning("Grupo não encontrado ou sem permissões ativas. GroupId={GroupId}", groupId);
                    throw new InvalidOperationException("Grupo não encontrado ou sem permissões ativas.");
                }

                // 3️⃣ Criar vínculo User ↔ Group se não existir
                var alreadyInGroup = await Context.UserGroup
                    .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId, cancellationToken);

                if (!alreadyInGroup)
                {
                    await Context.UserGroup.AddAsync(new UserGroup
                    {
                        UserId = userId,
                        GroupId = groupId,
                        IsActive = true,
                        AssignedAt = DateTime.UtcNow
                    }, cancellationToken);

                    Logger.LogInformation("Vínculo UserGroup criado: UserId={UserId}, GroupId={GroupId}", userId, groupId);
                }
                else
                {
                    Logger.LogInformation("Já existe vínculo UserGroup criado: UserId={UserId}, GroupId={GroupId}", userId, groupId);
                    throw new InvalidOperationException("Já existe vínculo do usuário com o grupo.");
                }

                // 4️⃣ Criar vínculos User ↔ Group ↔ Permissions em lote
                var existingPermissions = await Context.UsersGroupsPermissions
                    .Where(ugp => ugp.UserId == userId && ugp.GroupId == groupId)
                    .Select(ugp => ugp.PermissionId)
                    .ToListAsync(cancellationToken);

                var newLinks = activePermissions
                    .Where(pid => !existingPermissions.Contains(pid))
                    .Select(pid => new UsersGroupsPermissions
                    {
                        UserId = userId,
                        GroupId = groupId,
                        PermissionId = pid,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList();

                if (newLinks.Any())
                {
                    await Context.UsersGroupsPermissions.AddRangeAsync(newLinks, cancellationToken);
                    Logger.LogInformation("Criados {Count} vínculos UsersGroupsPermissions para UserId={UserId}, GroupId={GroupId}",
                        newLinks.Count, userId, groupId);
                }

                // 5️⃣ Salvar alterações
                await Context.SaveChangesAsync(cancellationToken);
                Logger.LogInformation("Alterações salvas com sucesso para UserId={UserId}", userId);

                // Retornar usuário atualizado já com grupos e permissões
                return await Context.Users
                    .Include(u => u.UserGroups)
                        .ThenInclude(ug => ug.Group)
                    .Include(u => u.UsersGroupsPermissions)
                        .ThenInclude(ugp => ugp.Permission)
                    .FirstAsync(u => u.Id == userId, cancellationToken);
            }
            catch (DbUpdateException ex)
            {
                Logger.LogError(ex, "Erro ao salvar alterações no banco. UserId={UserId}, GroupId={GroupId}", userId, groupId);
                throw new InvalidOperationException($"Erro ao salvar alterações: {ex.InnerException?.Message ?? ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Erro inesperado ao adicionar grupo ao usuário. UserId={UserId}, GroupId={GroupId}", userId, groupId);
                throw;
            }
        }

        
        // Atualiza o status de todas as permissões do usuário com base no status do usuário
        public async Task<User> UpdateUserGroupsPermissions(User user, CancellationToken cancellationToken)
        {
            using var transaction = await Context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                Logger.LogInformation("Iniciando atualização do usuário {UserId}", user.Id);
                // Atualiza UsersGroupsPermissions
                var entities = await Context.UsersGroupsPermissions
                    .Where(ugp => ugp.UserId == user.Id)
                    .ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    entity.IsActive = user.Status == StatusTypes.Active;
                    entity.UpdatedAt = DateTime.UtcNow;
                }

                Logger.LogInformation("Atualizando UserPermissions para usuário {UserId}", user.Id);

                // Atualiza UserPermissions
                var usersPermissions = await Context.UserPermissions
                    .Where(up => up.UserId == user.Id)
                    .ToListAsync(cancellationToken);

                foreach (var userPermission in usersPermissions)
                {
                    userPermission.IsActive = user.Status == StatusTypes.Active;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                }

                // Atualiza UserGroup
                var usersGroups = await Context.UserGroup
                    .Where(up => up.UserId == user.Id)
                    .ToListAsync(cancellationToken);

                foreach (var usersGroup in usersGroups)
                {
                    usersGroup.IsActive = user.Status == StatusTypes.Active;
                    usersGroup.UpdatedAt = DateTime.UtcNow;
                }                

                var updatedUser = Context.Users.Update(user);

                await Context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                Logger.LogInformation("Usuário {UserId} atualizado com sucesso", user.Id);
                return updatedUser.Entity;
            }
            catch (DomainException ex)
            {
                Logger.LogError(ex, "Erro de domínio ao atualizar usuário {UserId}", user.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "Erro inesperado ao atualizar usuário {UserId}", user.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        public async Task<bool> GetGroupNameAsync(string name, CancellationToken cancellationToken)
            => await Context.GroupPermissions.AnyAsync(g => g.Name == name, cancellationToken);

        public async Task<(List<User> users, int totalCount)> GetAllPagedAsync(int pageNumber, int pageSize, string sortBy, bool sortDescending, string searchTerm, CancellationToken cancellationToken)
        {
            var query = Context.Users.AsQueryable();

            if (!string.IsNullOrEmpty(searchTerm))
                query = query.Where(u => u.Username.Contains(searchTerm) || u.Email.Contains(searchTerm));

            if (!string.IsNullOrEmpty(sortBy))
                query = sortDescending ? query.OrderByDescending(u => EF.Property<object>(u, sortBy))
                                       : query.OrderBy(u => EF.Property<object>(u, sortBy));

            var totalCount = await query.CountAsync(cancellationToken);
            var users = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<User?> GetByEmailWithPermissionsAndGroupsAsync(string email, CancellationToken cancellationToken)
            => await Context.Users
                .Include(u => u.UserPermissions)
                .Include(u => u.UserGroups)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
            => await Context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await Context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAllUsersPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            var usersPermissions = await Context.UsersGroupsPermissions.Where(up => up.PermissionId == id).ToListAsync(cancellationToken);
            foreach (var userPermission in usersPermissions)
            {
                userPermission.IsActive = isActive;
                userPermission.UpdatedAt = DateTime.UtcNow;
            }
            //await _context.SaveChangesAsync(cancellationToken);
        }        


        //#region PaginatedList
        //public async Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy)
        //{
        //    if (page < 1) throw new ArgumentException("Page must be greater than 0", nameof(page));
        //    if (size < 1) throw new ArgumentException("Size must be greater than 0", nameof(size));

        //    try
        //    {
        //        var query = _context.Users
        //            .Include(u => u.Addresses)
        //            .Include(u => u.Phones)
        //            .AsNoTracking()
        //            .AsQueryable();

        //        if (ShouldIncludeCustomer(orderBy))
        //        {
        //            query = query.Include(u => u.Customers);
        //        }
        //        if (ShouldIncludeName(orderBy))
        //        {
        //            query = query.Include(u => u.Name);
        //        }
        //        if (ShouldIncludeAddress(orderBy))
        //        {
        //            query = query.Include(u => u.Addresses);
        //        }

        //        query = ApplyOrder(query, orderBy);

        //        var totalCount = await query.CountAsync();

        //        var items = await query
        //            .Skip((page - 1) * size)
        //            .Take(size)
        //            .ToListAsync();

        //        return new PaginatedList<User>(items, totalCount, page, size);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(ex.Message);
        //    }
        //}

        //private bool ShouldIncludeCustomer(string orderBy)
        //{
        //    return !string.IsNullOrEmpty(orderBy) &&
        //           orderBy.ToLower().Contains("customer");
        //}

        //private bool ShouldIncludeName(string orderBy)
        //{
        //    return !string.IsNullOrEmpty(orderBy) &&
        //           orderBy.ToLower().Contains("name");
        //}

        //private bool ShouldIncludeAddress(string orderBy)
        //{
        //    return !string.IsNullOrEmpty(orderBy) &&
        //           orderBy.ToLower().Contains("address");
        //}

        //private IQueryable<User> ApplyOrder(IQueryable<User> query, string orderBy)
        //{
        //    if (string.IsNullOrWhiteSpace(orderBy))
        //        return query.OrderBy(u => u.Username)
        //                   .ThenByDescending(u => u.Email);

        //    var orderParams = orderBy.ToLower().Split(',');
        //    var orderedQuery = query;
        //    bool firstOrder = true;

        //    foreach (var param in orderParams)
        //    {
        //        var trimmedParam = param.Trim();
        //        var parts = trimmedParam.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        //        var field = parts[0];
        //        var direction = parts.Length > 1 ? parts[1] : "asc";

        //        orderedQuery = ApplyOrderToField(orderedQuery, field, direction, ref firstOrder);
        //    }

        //    return orderedQuery;
        //}

        //private IQueryable<User> ApplyOrderToField(IQueryable<User> query, string field, string direction, ref bool firstOrder)
        //{
        //    switch (field.ToLower())
        //    {
        //        case "username":
        //            return ApplyDirection(query, u => u.Username, direction, ref firstOrder);
        //        case "email":
        //            return ApplyDirection(query, u => u.Email, direction, ref firstOrder);
        //        case "role":
        //            return ApplyDirection(query, u => u.Role, direction, ref firstOrder);
        //        case "status":
        //            return ApplyDirection(query, u => u.Status, direction, ref firstOrder);
        //        case "createdat":
        //            return ApplyDirection(query, u => u.CreatedAt, direction, ref firstOrder);
        //        case "updatedat":
        //            return ApplyDirection(query, u => u.UpdatedAt, direction, ref firstOrder);
        //        case "name":
        //            return ApplyDirection(query,
        //                u => u.Name != null ? u.Name.FirstName : string.Empty,
        //                direction, ref firstOrder);
        //        case "address.city":
        //            return ApplyDirection(query,
        //                u => u.Addresses.OrderBy(a => a.City).Select(a => a.City).FirstOrDefault() ?? string.Empty,
        //                direction, ref firstOrder);

        //        case "address.street":
        //            return ApplyDirection(query,
        //                u => u.Addresses.OrderBy(a => a.Street).Select(a => a.Street).FirstOrDefault() ?? string.Empty,
        //                direction, ref firstOrder);

        //        case "address.zipcode":
        //            return ApplyDirection(query,
        //                u => u.Addresses.OrderBy(a => a.ZipCode).Select(a => a.ZipCode).FirstOrDefault() ?? string.Empty,
        //                direction, ref firstOrder);

        //        case "address.geolocation":
        //            return ApplyDirection(query,
        //                u => u.Addresses
        //                    .OrderBy(a => a.Geolocation.Lat)
        //                    .Select(a => a.Geolocation != null ? a.Geolocation.Lat + "," + a.Geolocation.Long : "")
        //                    .FirstOrDefault() ?? string.Empty,
        //                direction, ref firstOrder);
        //        //error futuro
        //        case "customer":
        //            return ApplyDirection(query,
        //                u => u.Customers != null ? u.Customers.ToString() : string.Empty,
        //                direction, ref firstOrder);
        //        default:
        //            return firstOrder ?
        //                query.OrderBy(u => u.Username).ThenByDescending(u => u.Email) :
        //                ((IOrderedQueryable<User>)query).ThenBy(u => u.Username).ThenByDescending(u => u.Email);
        //    }
        //}

        //private IQueryable<User> ApplyDirection<TKey>(
        //    IQueryable<User> query,
        //    Expression<Func<User, TKey>> keySelector,
        //    string direction,
        //    ref bool firstOrder)
        //{
        //    if (firstOrder)
        //    {
        //        firstOrder = false;
        //        return direction == "desc" ?
        //            query.OrderByDescending(keySelector) :
        //            query.OrderBy(keySelector);
        //    }
        //    else
        //    {
        //        return direction == "desc" ?
        //            ((IOrderedQueryable<User>)query).ThenByDescending(keySelector) :
        //            ((IOrderedQueryable<User>)query).ThenBy(keySelector);
        //    }
        //}
        //#endregion

        //public IQueryable<User> Query()
        //{
        //    return _context.Users.AsQueryable();
        //}
    }
}

