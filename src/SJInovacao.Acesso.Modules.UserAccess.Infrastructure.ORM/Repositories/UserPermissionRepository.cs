using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using Polly;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserPermissionRepository
        : BaseRepository<UserPermission>, IUserPermissionRepository
    {
        private readonly IAsyncPolicy _readPolicy;

        public UserPermissionRepository(
            DefaultContext context,
            ILogger<UserPermissionRepository> logger,
            IAsyncPolicy? readPolicy = null)
            : base(context, logger)
        {
            _readPolicy = readPolicy ?? Policy.NoOpAsync();
        }

        public Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"AddUserPermission:{userId}:{permissionId}",
                async () =>
                {
                    var userExists = await _context.Users
                        .AsNoTracking()
                        .AnyAsync(u => u.Id == userId, ct);

                    if (!userExists)
                        throw new DomainException($"Usuário com ID {userId} não existe.");

                    var permissionExists = await _context.Permissions
                        .AsNoTracking()
                        .AnyAsync(p => p.Id == permissionId, ct);

                    if (!permissionExists)
                        throw new DomainException($"Permissão com ID {permissionId} não existe.");

                    var alreadyExists = await _context.UserPermissions
                        .AsNoTracking()
                        .AnyAsync(up => up.UserId == userId
                                     && up.PermissionId == permissionId, ct);

                    if (alreadyExists)
                        throw new DomainException("Este vínculo já existe.");

                    var userPermission = new UserPermission
                    {
                        UserId = userId,
                        PermissionId = permissionId,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.UserPermissions.Add(userPermission);
                    await _context.SaveChangesAsync(ct);
                });

        public Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"RemoveUserPermission:{userId}:{permissionId}",
                async () =>
                {
                    var userPermission = await _context.UserPermissions
                        .FirstOrDefaultAsync(up => up.UserId == userId
                                                 && up.PermissionId == permissionId, ct)
                        ?? throw new DomainException("Vínculo não encontrado.");

                    _context.UserPermissions.Remove(userPermission);
                    await _context.SaveChangesAsync(ct);
                });

        public Task UpdateStatusAsync(
            Guid userId, Guid permissionId, bool status, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"UpdateStatus:{userId}:{permissionId}:{status}",
                async () =>
                {
                    var userPermission = await _context.UserPermissions
                        .FirstOrDefaultAsync(up => up.UserId == userId
                                                 && up.PermissionId == permissionId, ct)
                        ?? throw new DomainException("Vínculo não encontrado.");

                    if (userPermission.IsActive == status)
                        return;

                    userPermission.IsActive = status;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                   
                    await _context.SaveChangesAsync(ct);
                });

        public Task<IEnumerable<Permission>> GetByUserIdAsync(
            Guid userId, CancellationToken ct)
            => ExecuteWithLoggingAsync<IEnumerable<Permission>>(
                $"GetPermissionsByUserId:{userId}",
                async () => await _readPolicy.ExecuteAsync(async () =>
                    await _context.UserPermissions
                        .AsNoTracking()
                        .Where(up => up.UserId == userId
                                  && up.IsActive
                                  && up.Permission != null)
                        .Select(up => up.Permission!)
                        .ToListAsync(ct)));

        public Task<IEnumerable<User>> GetByPermissionIdAsync(
            Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync<IEnumerable<User>>(
                $"GetUsersByPermissionId:{permissionId}",
                async () => await _readPolicy.ExecuteAsync(async () =>
                    await _context.Users
                        .AsNoTracking()
                        .Where(u => u.UserPermissions
                            .Any(up => up.PermissionId == permissionId && up.IsActive))
                        .ToListAsync(ct)));

        public Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct)
            => ExecuteWithLoggingAsync<IEnumerable<User>>(
                "GetAllUsersWithPermissions",
                async () => await _readPolicy.ExecuteAsync(async () =>
                    await _context.Users
                        .AsNoTracking()
                        .Where(u => u.UserPermissions.Any(up => up.IsActive))
                        .Include(u => u.UserPermissions.Where(up => up.IsActive))
                            .ThenInclude(up => up.Permission)
                        .ToListAsync(ct)));

        public Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"GetUserWithPermissions:{userId}",
                async () => await _readPolicy.ExecuteAsync(async () =>
                    await _context.Users
                        .AsNoTracking()
                        .Include(u => u.UserPermissions.Where(up => up.IsActive))
                            .ThenInclude(up => up.Permission)
                        .FirstOrDefaultAsync(u => u.Id == userId, ct)));

        public Task<bool> GetExistUserWithUsersPermissionsAsync(
            Guid userId, Guid permissionId, CancellationToken ct)
            => ExecuteWithLoggingAsync(
                $"CheckUserPermission:{userId}:{permissionId}",
                async () => await _readPolicy.ExecuteAsync(async () =>
                    await _context.UserPermissions
                        .AsNoTracking()
                        .AnyAsync(up => up.UserId == userId
                                     && up.PermissionId == permissionId
                                     && up.IsActive, ct)));
    }
}