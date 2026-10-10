using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(
            DefaultContext context,
            ILogger<PermissionRepository> logger)
            : base(context, logger)
        {
        }

        // =========================
        // LEITURA
        // =========================

        public Task<Permission?> GetByIdAsync(Guid id, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetById:{id}",
                async () => await _context.Permissions
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id, ct));

        public Task<List<Permission>> GetAllAsync(CancellationToken ct)
            => ExecuteWithLoggingAsync(
                "GetAll",
                async () => await _context.Permissions
                    .AsNoTracking()
                    .Include(p => p.UsersGroupsPermissions)
                        .ThenInclude(g => g.User)
                    .ToListAsync(ct));

        public Task<bool> GetNameAsync(string name, CancellationToken ct = default)
            => ExecuteWithLoggingAsync(
                $"GetName:{name}",
                async () => await _context.Permissions
                    .AsNoTracking()
                    .AnyAsync(p => p.Name == name, ct));

        // =========================
        // ESCRITA
        // =========================

        public Task<Permission> CreateAsync(Permission permission, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"Create:{permission.Name}",
                async () =>
                {
                    var exists = await _context.Permissions
                        .AnyAsync(p => p.Name == permission.Name, ct);

                    if (exists)
                        throw new DomainException("A permission with the same name already exists");

                    _context.Permissions.Add(permission);
                    await _context.SaveChangesAsync(ct);

                    return permission;
                });

        public Task<Permission> UpdateAsync(Permission permission, CancellationToken ct = default)
            => ExecuteWithLoggingAsync(
                $"Update:{permission.Id}",
                async () =>
                {
                    var exists = await _context.Permissions
                        .AnyAsync(p => p.Name == permission.Name && p.Id != permission.Id, ct);

                    if (exists)
                        throw new DomainException($"Já existe uma permissão com o nome '{permission.Name}'.");

                    _context.Permissions.Update(permission);
                    await _context.SaveChangesAsync(ct);

                    return permission;
                });

        public Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
            => ExecuteWithLoggingAsync(
                $"Delete:{id}",
                async () =>
                {
                    var entity = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.Id == id, ct);

                    if (entity is null)
                        return false;

                    _context.Permissions.Remove(entity);
                    await _context.SaveChangesAsync(ct);
                    return true;
                });

        public Task<bool> DesativarAsync(Guid id, CancellationToken ct = default)
            => ExecuteWithLoggingAsync(
                $"Desativar:{id}",
                async () =>
                {
                    var entity = await _context.Permissions
                        .FirstOrDefaultAsync(p => p.Id == id, ct);

                    if (entity is null)
                        return false;

                    entity.IsActive = false;
                    await _context.SaveChangesAsync(ct);
                    return true;
                });
    }
}