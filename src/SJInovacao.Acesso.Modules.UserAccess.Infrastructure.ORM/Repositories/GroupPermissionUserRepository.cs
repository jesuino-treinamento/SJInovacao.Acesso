using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupPermissionUserRepository : IGroupPermissionUserRepository
    {
        private readonly DefaultContext _context; 

        public GroupPermissionUserRepository(DefaultContext context)
        {
            _context = context;
        }

        public async Task<UserGroup?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {           
            return await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(g => g.GroupId == id, cancellationToken);
        }

        public IQueryable<UserGroup> Query()
        {
            return _context.Set<UserGroup>()
                .Include(g => g.Group.UsersGroupsPermissions)
                    .ThenInclude(ugp => ugp.Permission)
                .AsNoTracking(); 
        }

        public async Task<PaginatedList<UserGroup>> GetAllPaginatedAsync(int page, int size, string orderBy, CancellationToken cancellationToken)
        {
            var query = _context.UserGroup.Include(p => p.Group.UsersGroupsPermissions)
                .ThenInclude(ugp => ugp.Permission).AsQueryable();
            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);
            return new PaginatedList<UserGroup>(items, totalCount, page, size);
        }

        //// Métodos auxiliares para ordenação dinâmica
        //// Usando System.Linq.Expressions
        //public IQueryable<T> OrderByDynamic<T>(this IQueryable<T> source, string propertyName)
        //{
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var property = Expression.Property(parameter, propertyName);
        //    var lambda = Expression.Lambda(property, parameter);
        //    var method = typeof(Queryable).GetMethods()
        //        .First(m => m.Name == "OrderBy" && m.GetParameters().Length == 2)
        //        .MakeGenericMethod(typeof(T), property.Type);
        //    return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        //}

        //public IQueryable<T> OrderByDescendingDynamic<T>(this IQueryable<T> source, string propertyName)
        //{
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var property = Expression.Property(parameter, propertyName);
        //    var lambda = Expression.Lambda(property, parameter);
        //    var method = typeof(Queryable).GetMethods()
        //        .First(m => m.Name == "OrderByDescending" && m.GetParameters().Length == 2)
        //        .MakeGenericMethod(typeof(T), property.Type);
        //    return (IQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        //}

        //public IOrderedQueryable<T> ThenByDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
        //{
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var property = Expression.Property(parameter, propertyName);
        //    var lambda = Expression.Lambda(property, parameter);
        //    var method = typeof(Queryable).GetMethods()
        //        .First(m => m.Name == "ThenBy" && m.GetParameters().Length == 2)
        //        .MakeGenericMethod(typeof(T), property.Type);
        //    return (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        //}

        //public IOrderedQueryable<T> ThenByDescendingDynamic<T>(this IOrderedQueryable<T> source, string propertyName)
        //{
        //    var parameter = Expression.Parameter(typeof(T), "x");
        //    var property = Expression.Property(parameter, propertyName);
        //    var lambda = Expression.Lambda(property, parameter);
        //    var method = typeof(Queryable).GetMethods()
        //        .First(m => m.Name == "ThenByDescending" && m.GetParameters().Length == 2)
        //        .MakeGenericMethod(typeof(T), property.Type);
        //    return (IOrderedQueryable<T>)method.Invoke(null, new object[] { source, lambda })!;
        //}

        public async Task<List<UserGroup>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _context.Set<UserGroup>()
                .Include(g => g.Group.UsersGroupsPermissions)
                    .ThenInclude(ugp => ugp.Permission)
                //.AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            var junction = await _context.Set<UserGroup>()
                .FirstOrDefaultAsync(j => j.GroupId == groupId && j.UserId == permissionId, ct);

            if (junction != null)
            {
                _context.Set<UserGroup>().Remove(junction);
                await _context.SaveChangesAsync(ct);
            }
        }
    }
}
