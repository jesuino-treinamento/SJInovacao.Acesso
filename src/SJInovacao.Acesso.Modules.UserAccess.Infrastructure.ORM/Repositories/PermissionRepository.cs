using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class PermissionRepository : BaseRepository<Permission>, IPermissionRepository
    {
        public PermissionRepository(DefaultContext context, ILogger<PermissionRepository> logger)
            : base(context, logger)
        {
        }

        public async Task<Permission?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
        {
            _logger.LogDebug("Buscando permission por Id: {PermissionId}", id);

            var permission = await _context.Permissions
                .FindAsync(new object[] { id }, cancellationToken);

            if (permission is null)
                _logger.LogWarning("Permission não encontrada. Id: {PermissionId}", id);

            return permission;
        }

        public async Task<List<Permission>> GetAllAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug("Buscando todas as permissions");

            var permissions = await _context.Permissions
                .Include(p => p.UsersGroupsPermissions)
                    .ThenInclude(g => g.User)
                .ToListAsync(cancellationToken);

            _logger.LogInformation("Total de permissions encontradas: {Count}", permissions.Count);

            return permissions;
        }

        public async Task<Permission> CreateAsync(Permission permission, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Criando permission. Nome: {Name}", permission.Name);

            return await ExecuteWithLoggingAsync(
                "CreatePermission",
                async () =>
                {
                    var exist = await GetNameAsync(permission.Name, cancellationToken);
                    if (exist)
                        throw new DomainException("A permission with the same name already exists");

                    _context.Permissions.Add(permission);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Permission criada. Id: {PermissionId}", permission.Id);
                    return permission;
                });
        }

        public async Task<Permission> UpdateAsync(Permission permission, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Atualizando permission. Id: {PermissionId}", permission.Id);

            return await ExecuteWithLoggingAsync(
                "UpdatePermission",
                async () =>
                {
                    var exist = await _context.Permissions
                        .AnyAsync(p => p.Name == permission.Name && p.Id != permission.Id, cancellationToken);

                    if (exist)
                        throw new DomainException($"Já existe uma permissão com o nome '{permission.Name}'.");

                    _context.Permissions.Update(permission);
                    await _context.SaveChangesAsync(cancellationToken);

                    _logger.LogInformation("Permission atualizada. Id: {PermissionId}", permission.Id);
                    return permission;
                });
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Deletando permission. Id: {PermissionId}", id);

            return await ExecuteWithLoggingAsync(
                "DeletePermission",
                async () =>
                {
                    var entity = await GetByIdAsync(id, cancellationToken);
                    if (entity == null)
                    {
                        _logger.LogWarning("Permission não encontrada. Id: {PermissionId}", id);
                        return false;
                    }

                    _context.Permissions.Remove(entity);
                    await _context.SaveChangesAsync(cancellationToken);
                    return true;
                });
        }

        public async Task<bool> DesativarAsync(Guid id, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Desativando permission. Id: {PermissionId}", id);

            return await ExecuteWithLoggingAsync(
                "DesativarPermission",
                async () =>
                {
                    var entity = await GetByIdAsync(id, cancellationToken);
                    if (entity == null)
                    {
                        _logger.LogWarning("Permission não encontrada. Id: {PermissionId}", id);
                        return false;
                    }

                    entity.IsActive = false;
                    await _context.SaveChangesAsync(cancellationToken);
                    return true;
                });
        }

        public async Task<bool> GetNameAsync(string name, CancellationToken cancellationToken = default)
            => await _context.Permissions.AnyAsync(p => p.Name == name, cancellationToken);
    }
}