
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DefaultContext _context;
        public UserRepository(DefaultContext context)
        {
            _context = context;
        }
        public async Task<User> CreateAsync(User user, CancellationToken cancellationToken = default)
        {
            var exists = await _context.Users
                .AnyAsync(u => u.Email == user.Email || u.Username == user.Username, cancellationToken);

            if (exists)
                throw new DomainException("A user with the same email or username already exists");

            await _context.Users.AddAsync(user, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return user;
        }

        public async Task<bool> GetGroupNameAsync(string name, CancellationToken cancellationToken)
        {
            var group = await _context.GroupPermissions.AnyAsync(g => g.Name == name, cancellationToken);
            if (!group)
                return false;
            return true;
        }
        public async Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .Include(u => u.Addresses)
                .Include(u => u.Phones)
                .AsSplitQuery()
                .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
        }
        public async Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                
                .FirstOrDefaultAsync(u => u.Email == email && u.Status == StatusTypes.Active, cancellationToken);
        }

        public async Task<User?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == name, cancellationToken);
        }
        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await GetByIdAsync(id, cancellationToken);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existingUser = await GetByIdAsync(id, cancellationToken);
            if (existingUser == null)
                return false;

            _context.Entry(existingUser).CurrentValues.SetValues(existingUser);
            existingUser.Status = StatusTypes.Inactive;
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            //return existingUser;
            return true;
        }

        public async Task<User> UpdateAsync(User user, CancellationToken cancellationToken = default)
        {
            var existingUser = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == user.Id, cancellationToken);

            if (existingUser == null)
            {
                throw new DomainException($"User with ID {user.Id} not found for update");
            }

            _context.Entry(existingUser).CurrentValues.SetValues(user);
            existingUser.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return existingUser;
        }

        public async Task<User> AddGroupToUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            // Carrega usuário com permissões para evitar problemas de tracking
            var user = await _context.Users
                .Include(u => u.Groups)
                .ThenInclude(p => p.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                throw new Exception("User not found");

            // Busca permissão
            var group = await _context.GroupPermissions.FindAsync(new object[] { groupId }, cancellationToken);

            if (group == null)
                throw new Exception("GroupUser not found");

            // Verifica se usuário já possui a permissão para evitar duplicidade
            if (!user.Groups.Contains(group))
            {
                user.Groups.Add(group);
                await _context.SaveChangesAsync(cancellationToken);
            }

            return user;
        }

        public async Task<User> RemoveGroupFromUserAsync(Guid userId, Guid groupId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
               .Include(u => u.Groups)
               .ThenInclude(p => p.Permissions)
               .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
                throw new Exception("User not found");

            var group = await _context.GroupPermissions.FindAsync(new object[] { groupId }, cancellationToken);

            if (group == null)
                throw new Exception("GroupUser not found");

            if (user.Groups.Contains(group))
            {
                user.Groups.Remove(group);
                await _context.SaveChangesAsync(cancellationToken);
            }
            
            return user;
        }

        public async Task<bool> ExistsWithEmailOrUsernameAsync(string email, string username, CancellationToken cancellationToken = default)
        {
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == email || u.Username == username, cancellationToken);
        }

        public async Task<(IEnumerable<User> Users, int totalCount)> GetAllAsync(int page, int size, string orderBy)
        {
            var query = _context.Users
             .Include(s => s.Customers)
                 .ThenInclude(c => c.User)
             .AsQueryable();

            query = orderBy.ToLower() switch
            {
                "saledate" => query.OrderByDescending(s => s.Customers),
                "total_amount" => query.OrderByDescending(s => s.Id),
                "status" => query.OrderBy(s => s.Status),
                _ => query.OrderByDescending(s => s.Customers)
            };

            var totalCount = await query.CountAsync();

            var users = await query
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync();

            return (users, totalCount);
        }

        #region PaginatedList
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

                if (ShouldIncludeCustomer(orderBy))
                {
                    query = query.Include(u => u.Customers);
                }
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
                //error futuro
                case "customer":
                    return ApplyDirection(query,
                        u => u.Customers != null ? u.Customers.ToString() : string.Empty,
                        direction, ref firstOrder);
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

        public IQueryable<User> Query()
        {
            return _context.Users.AsQueryable();
        }

        public async Task<User?> GetGroupToUserAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.Groups)
                    .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            return user;
        }

        public async Task<(List<User> users, int totalCount)> GetAllPagedAsync(
        int pageNumber,
        int pageSize,
        string sortBy,
        bool sortDescending,
        string searchTerm,
        CancellationToken cancellationToken)
        {
            var query = _context.Users
                .Include(u => u.Name)
                .Include(u => u.Document)
                .Include(u => u.Phones)
                .Include(u => u.Addresses)
                .AsQueryable();

            // Aplicar filtro de busca (case-insensitive e com EF.Functions.Like)
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchTermLower = searchTerm.ToLower();

                query = query.Where(u =>
                    EF.Functions.Like(u.Username.ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Email.ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Name.FirstName.ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Name.LastName.ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Document.Number.ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Document.PersonType.ToString().ToLower(), $"%{searchTermLower}%") ||
                    u.Addresses.Any(a =>
                        EF.Functions.Like(a.Street.ToLower(), $"%{searchTermLower}%") ||
                        EF.Functions.Like(a.City.ToLower(), $"%{searchTermLower}%") ||
                        EF.Functions.Like(a.State.ToLower(), $"%{searchTermLower}%") ||
                        EF.Functions.Like(a.ZipCode.ToLower(), $"%{searchTermLower}%")) ||
                    u.Phones.Any(p =>
                        EF.Functions.Like(p.Number.ToLower(), $"%{searchTermLower}%")) ||
                    EF.Functions.Like(u.Status.ToString().ToLower(), $"%{searchTermLower}%") ||
                    EF.Functions.Like(u.Role.ToString().ToLower(), $"%{searchTermLower}%")
                );
            }           

            // Aplicar ordenação dinâmica com mapeamento seguro
            var propertyMappings = new Dictionary<string, Func<IQueryable<User>, IOrderedQueryable<User>>>
            {
                ["Username"] = q => sortDescending ? q.OrderByDescending(u => u.Username) : q.OrderBy(u => u.Username),
                ["Email"] = q => sortDescending ? q.OrderByDescending(u => u.Email) : q.OrderBy(u => u.Email),
                ["Name"] = q => sortDescending
                    ? q.OrderByDescending(u => u.Name.LastName).ThenByDescending(u => u.Name.FirstName)
                    : q.OrderBy(u => u.Name.LastName).ThenBy(u => u.Name.FirstName),
                ["Status"] = q => sortDescending ? q.OrderByDescending(u => u.Status) : q.OrderBy(u => u.Status),
                ["DocumentNumber"] = q => sortDescending
                    ? q.OrderByDescending(u => u.Document.Number)
                    : q.OrderBy(u => u.Document.Number),
                ["CreatedAt"] = q => sortDescending
                    ? q.OrderByDescending(u => u.CreatedAt)
                    : q.OrderBy(u => u.CreatedAt)
            };

            

            // Verifica se o campo de ordenação existe no dicionário
            var sortField = propertyMappings.ContainsKey(sortBy) ? sortBy : "Username";
            query = propertyMappings[sortField](query);

            // Obter contagem total (antes da paginação)
            var totalCount = await query.CountAsync(cancellationToken);

            // Aplicar paginação
            var users = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return (users, totalCount);
        }

        public async Task<User?> GetByEmailWithPermissionsAndGroupsAsync(string email, CancellationToken cancellationToken)
        {
            return await _context.Set<User>()
                .Include(u => u.Permissions)
                .Include(u => u.Groups)
                    .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
        }

        public async Task<User?> GetByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
        {
            //return await _context.Set<User>()
            //    .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken && u.RefreshTokenExpiry > DateTime.UtcNow, cancellationToken);
            return await _context.Users
                .Include(u => u.Permissions)
                .Include(u => u.UserGroups)
                    .ThenInclude(ug => ug.Group)
                        .ThenInclude(g => g.Permissions)
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, cancellationToken);
        }

        public async Task UpdateRefreshTokenAsync(Guid userId, string refreshToken, DateTime expiry, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiry = expiry;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
        {
            var user = await _context.Set<User>().FindAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await _context.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
