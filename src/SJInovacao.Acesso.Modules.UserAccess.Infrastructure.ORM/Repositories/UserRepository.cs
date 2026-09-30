
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
//using Microsoft.EntityFrameworkCore;
//using System.Linq;
//using System.Linq.Expressions;

//namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
//{
//    public class UserRepository : IUserRepository
//    {
//        private readonly DefaultContext _context;
//        public UserRepository(DefaultContext context)
//        {
//            _context = context;
//        }
//        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
//        {
//            var exists = await _context.Users
//                .AnyAsync(u => u.Email == user.Email || u.Username == user.Username, cancellationToken);

//            if (exists)
//                throw new DomainException("A user with the same email or username already exists");

//            await _context.Users.AddAsync(user, cancellationToken);
//            await _context.SaveChangesAsync(cancellationToken);
//            return user;
//        }

//        public async Task<bool> GetGroupNameAsync(string name, CancellationToken cancellationToken)
//        {
//            var group = await _context.GroupPermissions.AnyAsync(g => g.Name == name, cancellationToken);
//            if (!group)
//                return false;
//            return true;
//        }
//        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
//        {
//            return await _context.Users
//                .Include(u => u.Addresses)
//                .Include(u => u.Phones)
//                .AsSplitQuery()
//                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
//        }
//        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
//        {
//            return await _context.Users

//                .FirstOrDefaultAsync(u => u.Email == email && u.Status == StatusTypes.Active, cancellationToken);
//        }

//        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
//        {
//            return await _context.Users
//                .FirstOrDefaultAsync(u => u.Username == name, cancellationToken);
//        }
//        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
//        {
//            var user = await GetByIdAsync(id, cancellationToken);
//            if (user == null)
//                return false;

//            _context.Users.Remove(user);
//            await _context.SaveChangesAsync(cancellationToken);
//            return true;
//        }

//        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
//        {
//            var existingUser = await GetByIdAsync(id, cancellationToken);
//            if (existingUser == null)
//                return false;

//            _context.Entry(existingUser).CurrentValues.SetValues(existingUser);
//            existingUser.Status = StatusTypes.Inactive;
//            existingUser.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync(cancellationToken);
//            //return existingUser;
//            return true;
//        }

//        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
//        {
//            var existingUser = await _context.Users
//                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

//            if (existingUser == null)
//            {
//                throw new DomainException($"User with ID {user.Id} not found for update");
//            }

//            _context.Entry(existingUser).CurrentValues.SetValues(user);
//            existingUser.UpdatedAt = DateTime.UtcNow;

//            await _context.SaveChangesAsync(cancellationToken);
//            return existingUser;
//        }

//        public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
//        {
//            // Carrega usuário com grupos e permissões
//            var user = await _context.Users
//                .Include(u => u.UserGroups)
//                    .ThenInclude(ug => ug.Group)
//                        .ThenInclude(g => g.GroupsPermissions)
//                            .ThenInclude(gp => gp.Permission)
//                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//            if (user == null)
//                throw new Exception("User not found");

//            // Busca grupo
//            var group = await _context.GroupPermissions
//                .Include(g => g.GroupsPermissions)
//                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

//            if (group == null)
//                throw new Exception("Group not found");

//            // Verifica se já existe vínculo User -> Group
//            var existingUserGroup = user.UserGroups.FirstOrDefault(ug => ug.GroupId == groupId);

//            if (existingUserGroup == null)
//            {
//                user.UserGroups.Add(new UserGroup
//                {
//                    UserId = userId,
//                    User = user,
//                    Group = group,
//                    GroupId = groupId,
//                    IsActive = true,
//                    AssignedAt = DateTime.UtcNow
//                });

//                // Cria permissões individuais para o usuário dentro do grupo
//                foreach (var permission in _context.Permissions.Take(2)) // exemplo: adiciona algumas permissões
//                {
//                    if (!group.GroupsPermissions.Any(gp => gp.PermissionId == permission.Id && gp.UserId == userId))
//                    {
//                        group.GroupsPermissions.Add(new GroupsPermissions
//                        {
//                            UserId = userId,              // ✅ ESSENCIAL
//                            GroupId = groupId,
//                            PermissionId = permission.Id,
//                            IsActive = true,
//                            CreatedAt = DateTime.UtcNow
//                        });
//                    }
//                }

//                await _context.SaveChangesAsync(cancellationToken);
//            }

//            return user;
//        }

//        public async Task UpdateGroupPermissionAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
//        {
//            // Busca o vínculo existente
//            var groupPermission = await _context.GroupsPermissions
//                .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);

//            if (groupPermission == null)
//                throw new Exception("Permissão não encontrada para este grupo");

//            // Atualiza os campos desejados
//            groupPermission.IsActive = isActive;
//            groupPermission.UpdatedAt = DateTime.UtcNow;

//            // Salva alterações
//            await _context.SaveChangesAsync(ct);
//        }


//        public async Task<User> RemoveGroupFromUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
//        {
//            var user = await _context.Users
//                .Include(u => u.UserGroups)
//                    .ThenInclude(ug => ug.Group)
//                        .ThenInclude(g => g.Permissions)
//                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//            if (user == null)
//                throw new Exception("User not found");

//            var group = await _context.GroupPermissions
//                .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

//            if (group == null)
//                throw new Exception("Group not found");

//            // Localiza o vínculo UserGroup
//            var userGroup = user.UserGroups.FirstOrDefault(ug => ug.GroupId == groupId);

//            if (userGroup != null)
//            {
//                user.UserGroups.Remove(userGroup);
//                await _context.SaveChangesAsync(cancellationToken);
//            }

//            return user;
//        }


//        public async Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default)
//        {
//            return await _context.Users
//                .AsNoTracking()
//                .AnyAsync(u => u.Email == email || u.Username == username, cancellationToken);
//        }

//        public async Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy)
//        {
//            var query = _context.Users
//             .Include(s => s.Customers)
//                 .ThenInclude(c => c.User)
//             .AsQueryable();

//            query = orderBy.ToLower() switch
//            {
//                "saledate" => query.OrderByDescending(s => s.Customers),
//                "total_amount" => query.OrderByDescending(s => s.Id),
//                "status" => query.OrderBy(s => s.Status),
//                _ => query.OrderByDescending(s => s.Customers)
//            };

//            var totalCount = await query.CountAsync();

//            var users = await query
//                .Skip((page - 1) * size)
//                .Take(size)
//                .ToListAsync();

//            return (users, totalCount);
//        }

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

//        public async Task<User?> GetGroupToUserAsync(Guid userId, CancellationToken cancellationToken)
//        {
//            var user = await _context.Users
//                        .AsNoTracking()
//                        .Include(u => u.UserGroups)
//                            .ThenInclude(g => g.Group.Permissions)
//                        .Include(u => u.UserPermissions)
//                            .ThenInclude(up => up.Permission)
//                        .Include(u => u.Phones)
//                        .Include(u => u.Addresses)
//                        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

//            return user;
//        }

//        public async Task<(List<User> users, int totalCount)> GetAllPagedAsync(
//        int pageNumber,
//        int pageSize,
//        string sortBy,
//        bool sortDescending,
//        string searchTerm,
//        CancellationToken cancellationToken)
//        {
//            var query = _context.Users
//                .Include(u => u.Name)
//                .Include(u => u.Document)
//                .Include(u => u.Phones)
//                .Include(u => u.Addresses)
//                .AsQueryable();

//            // Aplicar filtro de busca (case-insensitive e com EF.Functions.Like)
//            if (!string.IsNullOrWhiteSpace(searchTerm))
//            {
//                var searchTermLower = searchTerm.ToLower();

//                query = query.Where(u =>
//                    EF.Functions.Like(u.Username.ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Email.ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Name.FirstName.ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Name.LastName.ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Document.Number.ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Document.PersonType.ToString().ToLower(), $"%{searchTermLower}%") ||
//                    u.Addresses.Any(a =>
//                        EF.Functions.Like(a.Street.ToLower(), $"%{searchTermLower}%") ||
//                        EF.Functions.Like(a.City.ToLower(), $"%{searchTermLower}%") ||
//                        EF.Functions.Like(a.State.ToLower(), $"%{searchTermLower}%") ||
//                        EF.Functions.Like(a.ZipCode.ToLower(), $"%{searchTermLower}%")) ||
//                    u.Phones.Any(p =>
//                        EF.Functions.Like(p.Number.ToLower(), $"%{searchTermLower}%")) ||
//                    EF.Functions.Like(u.Status.ToString().ToLower(), $"%{searchTermLower}%") ||
//                    EF.Functions.Like(u.Role.ToString().ToLower(), $"%{searchTermLower}%")
//                );
//            }           

//            // Aplicar ordenação dinâmica com mapeamento seguro
//            var propertyMappings = new Dictionary<string, Func<IQueryable<User>, IOrderedQueryable<User>>>
//            {
//                ["Username"] = q => sortDescending ? q.OrderByDescending(u => u.Username) : q.OrderBy(u => u.Username),
//                ["Email"] = q => sortDescending ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email),
//                ["Name"] = q => sortDescending
//                    ? q.OrderByDescending(u => u.Name.LastName).ThenByDescending(u => u.Name.FirstName)
//                    : q.OrderBy(u => u.Name.LastName).ThenBy(u => u.Name.FirstName),
//                ["Status"] = q => sortDescending ? q.OrderByDescending(u => u.Status) : q.OrderBy(u => u.Status),
//                ["DocumentNumber"] = q => sortDescending
//                    ? q.OrderByDescending(u => u.Document.Number)
//                    : q.OrderBy(u => u.Document.Number),
//                ["CreatedAt"] = q => sortDescending
//                    ? q.OrderByDescending(u => u.CreatedAt)
//                    : q.OrderBy(u => u.CreatedAt)
//            };



//            // Verifica se o campo de ordenação existe no dicionário
//            var sortField = propertyMappings.ContainsKey(sortBy) ? sortBy : "Username";
//            query = propertyMappings[sortField](query);

//            // Obter contagem total (antes da paginação)
//            var totalCount = await query.CountAsync(cancellationToken);

//            // Aplicar paginação
//            var users = await query
//                .Skip((pageNumber - 1) * pageSize)
//                .Take(pageSize)
//                .ToListAsync(cancellationToken);

//            return (users, totalCount);
//        }

//        public async Task<User?> GetByEmailWithPermissionsAndGroupsAsync(string email, CancellationToken cancellationToken)
//        {
//            return await _context.Set<User>()
//                .Include(u => u.UserPermissions)
//                .Include(u => u.UserGroups)
//                    .ThenInclude(g => g.Group.Permissions)
//                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
//        }

//        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
//        {
//            //return await _context.Set<User>()
//            //    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow, cancellationToken);
//            return await _context.Users
//                .Include(u => u.UserPermissions)
//                .Include(u => u.UserGroups)
//                    .ThenInclude(ug => ug.Group)
//                        .ThenInclude(g => g.Permissions)
//                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
//        }

//        public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken)
//        {
//            var user = await _context.Set<User>().FindAsync(userId);
//            if (user != null)
//            {
//                user.RefreshToken = refreshToken;
//                user.RefreshTokenExpiry = expiry;
//                await _context.SaveChangesAsync(cancellationToken);
//            }
//        }

//        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
//        {
//            var user = await _context.Set<User>().FindAsync(userId);
//            if (user != null)
//            {
//                user.RefreshToken = null;
//                user.RefreshTokenExpiry = null;
//                await _context.SaveChangesAsync(cancellationToken);
//            }
//        }

//        public async Task UpdateGroupPermissionAsync(Guid userId, Guid groupId, bool userIsActive, Guid? permissionId, bool? permissionIsActive, CancellationToken cancellationToken = default)
//        {

//            if (permissionId == null)
//            {

//                //var usergroupPermission = await _context.GroupPermissions
//                //    .FirstOrDefaultAsync(gp => gp.UserGroups.Where(u => u.UserId == userId).Any() && gp.Id == groupId, cancellationToken);
//                var usergroupPermission = await _context.UserGroup
//                    .FirstOrDefaultAsync(gp => gp.UserId == userId && gp.GroupId == groupId, cancellationToken);
//                if (usergroupPermission == null)
//                    throw new Exception("Permissão não encontrada para este grupo");

//                // Atualiza os campos desejados
//                usergroupPermission.IsActive = userIsActive;
//                usergroupPermission.UpdatedAt = DateTime.UtcNow;
//            }
//            else
//            {
//                // Busca o vínculo existente
//                var groupPermission = await _context.GroupsPermissions
//                    .FirstOrDefaultAsync(gp => gp.UserId == userId && gp.GroupId == groupId && gp.PermissionId == permissionId, cancellationToken);
//                if (groupPermission == null)
//                    throw new Exception("Permissão não encontrada para este grupo");

//                // Atualiza os campos desejados
//                groupPermission.IsActive = (bool)permissionIsActive;
//                groupPermission.UpdatedAt = DateTime.UtcNow;
//            }

//            // Salva alterações
//            await _context.SaveChangesAsync(cancellationToken);
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using System.Linq.Expressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DefaultContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(DefaultContext context, ILogger<UserRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
            => await _context.Users.FindAsync(new object[] { id }, cancellationToken);

        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default)
            => await _context.Users.AnyAsync(u => u.Email == email || u.Username == username, cancellationToken);

        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken)
            => await _context.Users.FirstOrDefaultAsync(u => u.Username == name, cancellationToken);

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user == null) return false;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user == null) return false;
            user.Deactivate();
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy)
        {
            var query = _context.Users.AsQueryable();
            var totalCount = await query.CountAsync();
            var users = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return (users, totalCount);
        }

        public async Task<PaginatedList<User>> GetAllPaginatedAsync(int page, int size, string orderBy)
        {
            var query = _context.Users.AsQueryable();
            var totalCount = await query.CountAsync();
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync();
            return new PaginatedList<User>(items, totalCount, page, size);
        }

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
            var user = await GetGroupToUserAsync(userId, cancellationToken);

            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
                var group = user.UserGroups.FirstOrDefault(g => g.GroupId == groupId);

            if (user.UsersGroupsPermissions.Any())
                _context.UsersGroupsPermissions.RemoveRange(user.UsersGroupsPermissions);

            if (group != null) user.UserGroups.Remove(group);
                await _context.SaveChangesAsync(cancellationToken);
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

            // Retorna lista de permissões atualizadas
            return updatedPermissions;
        }

        //public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        //{
        //    var user = await GetGroupToUserAsync(userId, cancellationToken);
        //    var group = await _context.GroupPermissions
        //        .Include(g => g.GroupsPermissions) // carrega vínculos de permissões
        //        .FirstOrDefaultAsync(g => g.Id == groupId, cancellationToken);

        //    if (user == null || group == null)
        //        throw new InvalidOperationException("Usuário ou grupo não encontrado.");

        //    // 1️⃣ Criar vínculo User ↔ Group
        //    var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id };
        //    user.UserGroups.Add(userGroup);

        //    await _context.UserGroup.AddAsync(userGroup);

        //    // 2️⃣ Criar vínculos User ↔ Group ↔ Permissions
        //    foreach (var gp in group.GroupsPermissions.Where(gp => gp.IsActive))
        //    {
        //        var ugp = new UsersGroupsPermissions
        //        {
        //            UserId = user.Id,
        //            GroupId = group.Id,
        //            PermissionId = gp.PermissionId,
        //            IsActive = true,
        //            CreatedAt = DateTime.UtcNow
        //        };



        //        await _context.UsersGroupsPermissions.AddAsync(ugp, cancellationToken);
        //    }

        //    // 3️⃣ Salvar alterações
        //    await _context.SaveChangesAsync(cancellationToken);

        //    return user;
        //}

        //public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        //{
        //    var user = await _context.Users
        //        .Include(u => u.UserGroups)
        //        .Include(u => u.UsersGroupsPermissions)
        //        .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        //    var group = await _context.GroupPermissions
        //        .Include(p => p.UserGroups)
        //            .ThenInclude(ug => ug.User)
        //        .Include(p => p.Permissions)
        //            .ThenInclude(p => p.GroupsPermissions)
        //        .Include(p => p.GroupsPermissions)
        //            .ThenInclude(gp => gp.Permission)
        //        .FirstOrDefaultAsync(p => p.Id == groupId, cancellationToken);

        //    if (user == null || group == null)
        //        throw new InvalidOperationException("Usuário ou grupo não encontrado.");

        //    // 1️⃣ Criar vínculo User ↔ Group
        //    if (!user.UserGroups.Any(ug => ug.GroupId == group.Id))
        //    {
        //        var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id };
        //        await _context.UserGroup.AddAsync(userGroup, cancellationToken);
        //    }



        //    // 2️⃣ Criar vínculos User ↔ Group ↔ Permissions
        //    foreach (var gp in group.GroupsPermissions.Where(gp => gp.IsActive))
        //    {
        //        if (!user.UsersGroupsPermissions.Any(ugp =>
        //            ugp.GroupId == group.Id && ugp.PermissionId == gp.PermissionId))
        //        {
        //            var ugp = new UsersGroupsPermissions
        //            {
        //                UserId = user.Id,
        //                GroupId = group.Id,
        //                PermissionId = gp.PermissionId,
        //                IsActive = true,
        //                CreatedAt = DateTime.UtcNow
        //            };                    
        //        }
        //        var exists = await _context.UsersGroupsPermissions
        //                                    .AnyAsync(x => x.UserId == user.Id &&
        //                                    x.GroupId == group.Id && x.PermissionId == gp.PermissionId,
        //                                    cancellationToken);

        //        if (!exists)
        //        {
        //            await _context.UsersGroupsPermissions.AddAsync(ugp, cancellationToken);
        //        }

        //    }

        //    // 3️⃣ Salvar alterações
        //    await _context.SaveChangesAsync(cancellationToken);

        //    return user;
        //}

        public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .Include(u => u.UserGroups)
                    .Include(u => u.UsersGroupsPermissions)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

                var group = await _context.GroupPermissions
                    .Include(p => p.UserGroups)
                        .ThenInclude(ug => ug.User)
                    .Include(p => p.Permissions)
                        .ThenInclude(p => p.GroupsPermissions)
                    .Include(p => p.GroupsPermissions)
                        .ThenInclude(gp => gp.Permission)
                    .FirstOrDefaultAsync(p => p.Id == groupId, cancellationToken);

                if (user == null || group == null)
                {
                    _logger.LogWarning("Usuário ou grupo não encontrado. UserId={UserId}, GroupId={GroupId}", userId, groupId);
                    throw new InvalidOperationException("Usuário ou grupo não encontrado.");
                }

                // 1️⃣ Criar vínculo User ↔ Group
                if (!user.UserGroups.Any(ug => ug.GroupId == group.Id))
                {
                    var userGroup = new UserGroup { UserId = user.Id, GroupId = group.Id };
                    await _context.UserGroup.AddAsync(userGroup, cancellationToken);
                    _logger.LogInformation("Vínculo UserGroup criado: UserId={UserId}, GroupId={GroupId}", user.Id, group.Id);
                }

                // 2️⃣ Criar vínculos User ↔ Group ↔ Permissions
                foreach (var gp in group.GroupsPermissions.Where(gp => gp.IsActive))
                {
                    var exists = await _context.UsersGroupsPermissions
                        .AnyAsync(x => x.UserId == user.Id &&
                                       x.GroupId == group.Id &&
                                       x.PermissionId == gp.PermissionId,
                                       cancellationToken);

                    if (!exists)
                    {
                        var ugp = new UsersGroupsPermissions
                        {
                            UserId = user.Id,
                            GroupId = group.Id,
                            PermissionId = gp.PermissionId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        await _context.UsersGroupsPermissions.AddAsync(ugp, cancellationToken);
                        _logger.LogInformation("Vínculo UsersGroupsPermissions criado: UserId={UserId}, GroupId={GroupId}, PermissionId={PermissionId}",
                            user.Id, group.Id, gp.PermissionId);
                    }
                }

                // 3️⃣ Salvar alterações
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Alterações salvas com sucesso para UserId={UserId}", user.Id);

                return user;
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
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiry;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await GetByIdAsync(userId, cancellationToken);
            if (user == null) throw new InvalidOperationException("Usuário não encontrado.");
            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task UpdateAllUsersPermissionStatusAsync(Guid id, bool isActive, CancellationToken cancellationToken)
        {
            var usersPermissions = await _context.UsersGroupsPermissions.Where(up => up.PermissionId == id).ToListAsync(cancellationToken);
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

