
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;

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
                "GetByIdUser",
                async () =>
                {
                    return await _context.Users
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
                return await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Phones)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
            
            });
        }

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
            "CreateUser",
            async () =>
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(cancellationToken);
                return user;
            });
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
           "UpdateUser",
           async () =>
           {
               _context.Users.Update(user);
               await _context.SaveChangesAsync(cancellationToken);
               return user;
           });
        }

        public async Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default)
            => await _context.Users.AnyAsync(u => u.Email == email || u.Username == username, cancellationToken);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == name, cancellationToken);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
            "DeleteUser",
            async () =>
            {
                var user = await GetByIdAsync(id, cancellationToken);
                if (user == null) return false;
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            });
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
            "DesativarUser",
            async () =>
            {
                var user = await GetByIdAsync(id, cancellationToken);
                if (user == null) return false;
                user.Deactivate();
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            });
        }

        public async Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy)
        {
            var query = _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Phones)
                .AsQueryable();
            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (users, totalCount);
        }

        #region PaginatedList
        //public async Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy)
        //{
        //    try
        //    {
        //        return await ExecuteWithLoggingAsync(
        //           "GetAllPaginated",
        //           async () =>
        //           {
        //               if (page < 1) throw new ArgumentException("Page must be greater than 0", nameof(page));
        //               if (size < 1) throw new ArgumentException("Size must be greater than 0", nameof(size));

        //               var query = Context.Users
        //                    .Include(u => u.Addresses)
        //                    .Include(u => u.Phones)
        //                    .Select(u => new
        //                    {
        //                        User = u,
        //                        Street = u.Addresses.Select(a => a.Street).FirstOrDefault(),
        //                        Neighborhood = u.Addresses.Select(a => a.Neighborhood).FirstOrDefault(),
        //                        AddressCity = u.Addresses.Select(a => a.City).FirstOrDefault(),
        //                        Username = u.Username,
        //                        Email = u.Email,
        //                        CreatedAt = u.CreatedAt
        //                    })
        //                    .AsNoTracking();

        //               query = query.OrderBy(NormalizeOrderBy(orderBy));

        //               var totalCount = await query.CountAsync();

        //               var items = await query
        //                   .Skip((page - 1) * size)
        //                   .Take(size)
        //                   .Select(x => x.User) // converte para List<User>
        //                   .ToListAsync();

        //               return new PaginatedList<User>(items, totalCount, page, size);
        //           });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new DomainException(ex.Message);
        //    }
        //}

        public async Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy)
        {
            if (page < 1) throw new ArgumentException("Page must be greater than 0", nameof(page));
            if (size < 1) throw new ArgumentException("Size must be greater than 0", nameof(size));

            try
            {
                var query = _context.Users
                    .Include(u => u.Addresses)
                    .Include(u => u.Phones)
                    .AsNoTracking()
                    .AsQueryable();

                //if (ShouldIncludeCustomer(orderBy))
                //{
                //    query = query.Include(u => u.Customers);
                //}
                if (ShouldIncludeName(orderBy))
                {
                    query = query.Include(u => u.Name);
                }
                if (ShouldIncludeAddress(orderBy))
                {
                    query = query.Include(u => u.Addresses);
                }

                query = ApplyOrder(query, orderBy);

                var totalCount = await query.CountAsync();

                var items = await query
                    .Skip((page - 1) * size)
                    .Take(size)
                    .ToListAsync();

                return new PaginatedList<User>(items, totalCount, page, size);
            }
            catch (Exception ex)
            {
                throw new DomainException(ex.Message);
            }
        }

        private string NormalizeOrderBy(string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return "Username asc, Email desc";

            var normalizedClauses = new List<string>();

            foreach (var clause in orderBy.Split(','))
            {
                var parts = clause.Trim().Split(' ');
                var field = parts[0].ToLower();
                var direction = parts.Length > 1 ? parts[1].ToLower() : "asc";

                // Mapeia nomes amigáveis para campos projetados
                field = field switch
                {
                    "address.street" or "street" => "Street",
                    "address.neighborhood" or "neighborhood" => "Neighborhood",
                    "address.city" or "city" => "AddressCity",
                    "username" => "Username",
                    "email" => "Email",
                    "createdat" => "CreatedAt",
                    _ => field
                };

                normalizedClauses.Add($"{field} {direction}");
            }

            return string.Join(", ", normalizedClauses);
        }


        private bool ShouldIncludeCustomer(string orderBy)
        {
            return !string.IsNullOrEmpty(orderBy) &&
                   orderBy.ToLower().Contains("customer");
        }

        private bool ShouldIncludeName(string orderBy)
        {
            return !string.IsNullOrEmpty(orderBy) &&
                   orderBy.ToLower().Contains("name");
        }

        private bool ShouldIncludeAddress(string orderBy)
        {
            return !string.IsNullOrEmpty(orderBy) &&
                   orderBy.ToLower().Contains("address");
        }

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

        //private IQueryable<User> ApplyOrder(IQueryable<User> query, string orderBy)
        //{
        //    if (string.IsNullOrWhiteSpace(orderBy))
        //        return query.OrderBy("Username asc, Email desc");

        //    return query.OrderBy(orderBy);
        //}

        private IQueryable<User> ApplyOrder(IQueryable<User> query, string orderBy)
        {
            if (string.IsNullOrWhiteSpace(orderBy))
                return query.OrderBy(u => u.Username)
                           .ThenByDescending(u => u.Email);

            var orderParams = orderBy.ToLower().Split(',');
            var orderedQuery = query;
            bool firstOrder = true;

            foreach (var param in orderParams)
            {
                var trimmedParam = param.Trim();
                var parts = trimmedParam.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var field = parts[0];
                var direction = parts.Length > 1 ? parts[1] : "asc";

                orderedQuery = ApplyOrderToField(orderedQuery, field, direction, ref firstOrder);
            }

            return orderedQuery;
        }

        private IQueryable<User> ApplyOrderToField(IQueryable<User> query, string field, string direction, ref bool firstOrder)
        {
            switch (field.ToLower())
            {
                case "username":
                    return ApplyDirection(query, u => u.Username, direction, ref firstOrder);
                case "email":
                    return ApplyDirection(query, u => u.Email, direction, ref firstOrder);
                case "role":
                    return ApplyDirection(query, u => u.Role, direction, ref firstOrder);
                case "status":
                    return ApplyDirection(query, u => u.Status, direction, ref firstOrder);
                case "createdat":
                    return ApplyDirection(query, u => u.CreatedAt, direction, ref firstOrder);
                case "updatedat":
                    return ApplyDirection(query, u => u.UpdatedAt, direction, ref firstOrder);
                case "name":
                    return ApplyDirection(query,
                        u => u.Name != null ? u.Name.FirstName : string.Empty,
                        direction, ref firstOrder);
                case "address.city":
                    return ApplyDirection(query,
                        u => u.Addresses.OrderBy(a => a.City).Select(a => a.City).FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                case "address.street":
                    return ApplyDirection(query,
                        u => u.Addresses.OrderBy(a => a.Street).Select(a => a.Street).FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                case "address.neighborhood":
                    return ApplyDirection(query,
                        u => u.Addresses.OrderBy(a => a.Neighborhood).Select(a => a.Neighborhood).FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                case "address.zipcode":
                    return ApplyDirection(query,
                        u => u.Addresses.OrderBy(a => a.ZipCode).Select(a => a.ZipCode).FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                case "address.geolocation":
                    return ApplyDirection(query,
                        u => u.Addresses
                            .OrderBy(a => a.Geolocation.Lat)
                            .Select(a => a.Geolocation != null ? a.Geolocation.Lat + "," + a.Geolocation.Long : "")
                            .FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                case "phone":
                    return ApplyDirection(query,
                        u => u.Phones.OrderBy(a => a.Number).Select(a => a.Number).FirstOrDefault() ?? string.Empty,
                        direction, ref firstOrder);

                //error futuro
                //case "customer":
                //    return ApplyDirection(query,
                //        u => u.Customers != null ? u.Customers.ToString() : string.Empty,
                //        direction, ref firstOrder);
                default:
                    return firstOrder ?
                        query.OrderBy(u => u.Username).ThenByDescending(u => u.Email) :
                        ((IOrderedQueryable<User>)query).ThenBy(u => u.Username).ThenByDescending(u => u.Email);
            }
        }

        private IQueryable<User> ApplyDirection<TKey>(
            IQueryable<User> query,
            Expression<Func<User, TKey>> keySelector,
            string direction,
            ref bool firstOrder)
        {
            if (firstOrder)
            {
                firstOrder = false;
                return direction == "desc" ?
                    query.OrderByDescending(keySelector) :
                    query.OrderBy(keySelector);
            }
            else
            {
                return direction == "desc" ?
                    ((IOrderedQueryable<User>)query).ThenByDescending(keySelector) :
                    ((IOrderedQueryable<User>)query).ThenBy(keySelector);
            }
        }
        #endregion

        public IQueryable<User> Query() => _context.Users.AsQueryable();

        public async Task<User?> GetGroupToUserAsync(Guid userId, CancellationToken cancellationToken)
            => await _context.Users  
            .Include(u => u.UserPermissions)
                .ThenInclude(g => g.Permission)
            .Include(u => u.UserGroups)
                .ThenInclude(g => g.Group.Permissions)
            .Include(u => u.UsersGroupsPermissions)
                .ThenInclude(g => g.Permission)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        public async Task<User> RemoveGroupFromUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
            "RemoveGroupFromUser",
            async () =>
            {
                var user = await GetGroupToUserAsync(userId, cancellationToken);

                if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
                var group = user.UserGroups.FirstOrDefault(g => g.UserId == userId && g.GroupId == groupId);

                if (user.UsersGroupsPermissions.Any())
                    _context.UsersGroupsPermissions.RemoveRange(user.UsersGroupsPermissions.Where(ugp => ugp.GroupId == groupId));

                if (group != null) user.UserGroups.Remove(group);
                await _context.SaveChangesAsync(cancellationToken);
                return user;
            });
        }

        public async Task<List<Permission>> UpdateGroupPermissionAsync(
        Guid userId, Guid groupId, bool userIsActive, List<Guid>? permissionIds, bool? permissionIsActive, CancellationToken cancellationToken = default)
        {
            return await ExecuteWithLoggingAsync(
            "UpdateGroupPermission",
            async () =>
            {
                var user = await GetGroupToUserAsync(userId, cancellationToken);
                if (user == null) throw new InvalidOperationException("Usuário não encontrado.");

                var group = user.UserGroups.FirstOrDefault(g => g.GroupId == groupId);
                if (group == null) throw new InvalidOperationException("Grupo não encontrado.");

                group.IsActive = userIsActive;

                var updatedPermissions = new List<Permission>();

                if (permissionIds == null || !permissionIds.Any())
                {
                    foreach (var ugp in group.User.UsersGroupsPermissions
                        .Where(p => p.GroupId == groupId && p.UserId == userId))
                    {
                        ugp.IsActive = userIsActive;
                        ugp.UpdatedAt = DateTime.UtcNow;
                        updatedPermissions.Add(ugp.Permission);
                    }

                    _context.UsersGroupsPermissions.UpdateRange(group.User.UsersGroupsPermissions);
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

                    _context.UsersGroupsPermissions.UpdateRange(group.User.UsersGroupsPermissions);
                }

                await _context.SaveChangesAsync(cancellationToken);

                return updatedPermissions;
            });
        }

        public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            return await ExecuteWithLoggingAsync(
            "AddGroupToUser",
            async () =>
            {
                try
                {
                    var user = await _context.Users
                    .Include(u => u.UserGroups)
                    .Include(u => u.UsersGroupsPermissions)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                    if (user == null)
                    {
                        _logger.LogWarning("Usuário não encontrado. UserId={UserId}", userId);
                        throw new InvalidOperationException("Usuário não encontrado.");
                    }

                    var activePermissions = await _context.GroupsPermissions
                        .Where(gp => gp.GroupId == groupId && gp.IsActive)
                        .Select(gp => gp.PermissionId)
                        .ToListAsync(cancellationToken);

                    if (!activePermissions.Any())
                    {
                        _logger.LogWarning("Grupo não encontrado ou sem permissões ativas. GroupId={GroupId}", groupId);
                        throw new InvalidOperationException("Grupo não encontrado ou sem permissões ativas.");
                    }

                    var alreadyInGroup = await _context.UserGroup
                        .AnyAsync(ug => ug.UserId == userId && ug.GroupId == groupId, cancellationToken);

                    if (!alreadyInGroup)
                    {
                        await _context.UserGroup.AddAsync(new UserGroup
                        {
                            UserId = userId,
                            GroupId = groupId,
                            IsActive = true,
                            AssignedAt = DateTime.UtcNow
                        }, cancellationToken);

                        _logger.LogInformation("Vínculo UserGroup criado: UserId={UserId}, GroupId={GroupId}", userId, groupId);
                    }
                    else
                    {
                        _logger.LogInformation("Já existe vínculo UserGroup criado: UserId={UserId}, GroupId={GroupId}", userId, groupId);
                        throw new InvalidOperationException("Já existe vínculo do usuário com o grupo.");
                    }

                    var existingPermissions = await _context.UsersGroupsPermissions
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
                        await _context.UsersGroupsPermissions.AddRangeAsync(newLinks, cancellationToken);
                        _logger.LogInformation("Criados {Count} vínculos UsersGroupsPermissions para UserId={UserId}, GroupId={GroupId}",
                            newLinks.Count, userId, groupId);
                    }

                    await _context.SaveChangesAsync(cancellationToken);
                    _logger.LogInformation("Alterações salvas com sucesso para UserId={UserId}", userId);

                    return await _context.Users
                        .Include(u => u.UserGroups)
                            .ThenInclude(ug => ug.Group)
                        .Include(u => u.UsersGroupsPermissions)
                            .ThenInclude(ugp => ugp.Permission)
                        .FirstAsync(u => u.Id == userId, cancellationToken);
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Erro ao salvar alterações no banco. UserId={UserId}, GroupId={GroupId}", userId, groupId);
                    throw new InvalidOperationException($"Erro ao salvar alterações: {ex.InnerException?.Message ?? ex.Message}", ex);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro inesperado ao adicionar grupo ao usuário. UserId={UserId}, GroupId={GroupId}", userId, groupId);
                    throw;
                }

            });            
        }

        public async Task<User> UpdateUserGroupsPermissions(User user, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);           
            return await ExecuteWithLoggingAsync(
            "UpdateUserGroupsPermissions",
            async () =>
            {
                try
                {
                    _logger.LogInformation("Iniciando atualização do usuário {UserId}", user.Id);
                    var entities = await _context.UsersGroupsPermissions
                        .Where(ugp => ugp.UserId == user.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var entity in entities)
                    {
                        entity.IsActive = user.Status == StatusTypes.Active;
                        entity.UpdatedAt = DateTime.UtcNow;
                    }

                    _logger.LogInformation("Atualizando UserPermissions para usuário {UserId}", user.Id);
                    var usersPermissions = await _context.UserPermissions
                        .Where(up => up.UserId == user.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var userPermission in usersPermissions)
                    {
                        userPermission.IsActive = user.Status == StatusTypes.Active;
                        userPermission.UpdatedAt = DateTime.UtcNow;
                    }
                    var usersGroups = await _context.UserGroup
                        .Where(up => up.UserId == user.Id)
                        .ToListAsync(cancellationToken);

                    foreach (var usersGroup in usersGroups)
                    {
                        usersGroup.IsActive = user.Status == StatusTypes.Active;
                        usersGroup.UpdatedAt = DateTime.UtcNow;
                    }

                    var updatedUser = _context.Users.Update(user);

                    await _context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);

                    _logger.LogInformation("Usuário {UserId} atualizado com sucesso", user.Id);
                    return updatedUser.Entity;
                }
                catch (DomainException ex)
                {
                    _logger.LogError(ex, "Erro de domínio ao atualizar usuário {UserId}", user.Id);
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex, "Erro inesperado ao atualizar usuário {UserId}", user.Id);
                    await transaction.RollbackAsync(cancellationToken);
                    throw;
                }

            });
           
        }

        public async Task<bool> GetGroupNameAsync(string name, CancellationToken cancellationToken)
            => await _context.GroupPermissions.AnyAsync(g => g.Name == name, cancellationToken);

        public async Task<(List<User> users, int totalCount)> GetAllPagedAsync(int pageNumber, int pageSize, string sortBy, bool sortDescending, string searchTerm, CancellationToken cancellationToken)
        {
            var query = _context.Users.AsQueryable();
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
            => await _context.Users
                .Include(u => u.UserPermissions)
                .Include(u => u.UserGroups)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);

        public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken)
        {
            await ExecuteWithLoggingAsync(
                "UpdateRefreshToken",
                async () =>
                {
                    var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                    if (user == null) throw new InvalidOperationException("Usuário não encontrado.");

                    user.RefreshToken = refreshToken;
                    user.RefreshTokenExpiry = expiry;

                    _context.Users.Update(user); // 🔑 garante que o EF rastreie a entidade
                    await _context.SaveChangesAsync(cancellationToken);
                });
        }


        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            await ExecuteWithLoggingAsync(
            "RevokeRefreshToken",
            async () =>
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
                if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await _context.SaveChangesAsync(cancellationToken);
            });
        }

        public async Task UpdateAllUsersPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            await ExecuteWithLoggingAsync(
            "UpdateAllUsersPermissionStatus",
            async () =>
            {
                var usersPermissions = await _context.UsersGroupsPermissions.Where(up => up.PermissionId == id).ToListAsync(cancellationToken);
                foreach (var userPermission in usersPermissions)
                {
                    userPermission.IsActive = isActive;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                }
            });
        }  
    }
}

