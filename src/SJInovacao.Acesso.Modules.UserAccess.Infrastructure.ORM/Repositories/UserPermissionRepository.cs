using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly DefaultContext _context;
        private readonly ILogger<UserPermissionRepository> _logger;

        public UserPermissionRepository(DefaultContext context, ILogger<UserPermissionRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            var entity = new UserPermission
            {
                UserId = userId,
                PermissionId = permissionId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Set<UserPermission>().AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            var entity = await _context.Set<UserPermission>()
                .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

            if (entity != null)
            {
                _context.Set<UserPermission>().Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                return await _context.Set<UserPermission>()
                    .Where(up => up.UserId == userId)
                    .Include(up => up.Permission)
                    .Select(up => up.Permission!)
                    .ToListAsync(ct);
            });
        }

        public async Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
        {
            var strategy = _context.Database.CreateExecutionStrategy();
            return await strategy.ExecuteAsync(async () =>
            {
                return await _context.Set<UserPermission>()
                    .Where(up => up.PermissionId == permissionId)
                    .Include(up => up.User)
                    .Select(up => up.User!)
                    .ToListAsync(ct);
            });
        }

        public async Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct)
        {
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                await strategy.ExecuteAsync(async () =>
                {
                    var entity = await _context.Set<UserPermission>()
                        .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

                    if (entity == null)
                        return;

                    var isActiveProp = entity.GetType().GetProperty("IsActive");
                    if (isActiveProp != null && isActiveProp.CanWrite)
                        isActiveProp.SetValue(entity, status);

                    var updatedAtProp = entity.GetType().GetProperty("UpdatedAt");
                    if (updatedAtProp != null && updatedAtProp.CanWrite)
                        updatedAtProp.SetValue(entity, DateTime.UtcNow);

                    _context.Set<UserPermission>().Update(entity);
                    await _context.SaveChangesAsync(ct);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao atualizar status da permissão do usuário {UserId} - {PermissionId}", userId, permissionId);
                throw;
            }
        }

        public async Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct)
        {
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();

                return await strategy.ExecuteAsync(async () =>
                {
                    var users = await _context.Users
                        .AsNoTracking()
                        .Include(u => u.UserPermissions)
                            .ThenInclude(up => up.Permission)
                        .ToListAsync(ct);

                    return users;
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao recuperar todos os usuários com permissões");
                throw;
            }
        }

        public async Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct)
        {
            try
            {
                var strategy = _context.Database.CreateExecutionStrategy();
                return await strategy.ExecuteAsync(async () =>
                {
                    return await _context.Users
                        .AsNoTracking()
                        .Include(u => u.UserPermissions!)
                            .ThenInclude(up => up.Permission)
                        .FirstOrDefaultAsync(u => u.Id == userId, ct);
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao recuperar usuário {UserId} com permissões", userId);
                throw;
            }
        }

        public async Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId,CancellationToken ct)
        {
            try
            {
                var existingUserPermission = await _context.Set<UserPermission>()
                    .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);//?? new();


                if(existingUserPermission == null)
                {
                    _logger.LogWarning("Usuário {UserId} não possui a permissão {PermissionId}", userId, permissionId);
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Falha ao recuperar usuário {UserId} com permissões", userId);
                throw;
            }
        }
    }
}