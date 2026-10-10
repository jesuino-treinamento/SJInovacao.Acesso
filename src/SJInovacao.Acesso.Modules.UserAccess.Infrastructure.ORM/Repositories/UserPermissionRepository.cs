//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Logging;
//using Polly;
//using System.Diagnostics;

//namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
//{
//    public class UserPermissionRepository : BaseRepository<UserPermission>, IUserPermissionRepository
//    {        
//        private readonly IAsyncPolicy _resiliencePolicy;

//        public UserPermissionRepository(
//            DefaultContext context,
//            ILogger<UserPermissionRepository> logger,
//            IAsyncPolicy? resiliencePolicy = null)
//            : base(context, logger)
//        {
//            _resiliencePolicy = resiliencePolicy ?? Policy.NoOpAsync();
//        }

//        public async Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("AddUserPermission {UserId} {PermissionId}", userId, permissionId))
//            {
//                try
//                {
//                    await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        _logger.LogInformation("➕ Adicionando permissão para usuário {UserId}", userId);

//                        var userExists = await _context.Users
//                            .AnyAsync(u => u.Id == userId, ct);

//                        if (!userExists)
//                        {
//                            _logger.LogWarning("⚠️ Usuário {UserId} não encontrado", userId);
//                            throw new InvalidOperationException($"Usuário com ID {userId} não existe.");
//                        }

//                        var permissionExists = await _context.Permissions
//                            .AnyAsync(p => p.Id == permissionId, ct);

//                        if (!permissionExists)
//                        {
//                            _logger.LogWarning("⚠️ Permissão {PermissionId} não encontrada", permissionId);
//                            throw new InvalidOperationException($"Permissão com ID {permissionId} não existe.");
//                        }

//                        var alreadyExists = await _context.UserPermissions
//                            .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

//                        if (alreadyExists)
//                        {
//                            _logger.LogWarning("⚠️ Vínculo já existe para usuário {UserId} e permissão {PermissionId}", userId, permissionId);
//                            throw new InvalidOperationException("Este vínculo já existe.");
//                        }

//                        var userPermission = new UserPermission
//                        {
//                            UserId = userId,
//                            PermissionId = permissionId,
//                            IsActive = true,
//                            CreatedAt = DateTime.UtcNow
//                        };

//                        _context.UserPermissions.Add(userPermission);
//                        await _context.SaveChangesAsync(ct);

//                        _logger.LogInformation("✅ Permissão adicionada com sucesso");
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao adicionar permissão: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("RemoveUserPermission {UserId} {PermissionId}", userId, permissionId))
//            {
//                try
//                {
//                    await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        _logger.LogInformation("🗑️ Removendo permissão de usuário");

//                        var userPermission = await _context.UserPermissions
//                            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

//                        if (userPermission == null)
//                        {
//                            _logger.LogWarning("⚠️ Vínculo não encontrado");
//                            throw new InvalidOperationException("Vínculo não encontrado.");
//                        }

//                        _context.UserPermissions.Remove(userPermission);
//                        await _context.SaveChangesAsync(ct);

//                        _logger.LogInformation("✅ Permissão removida com sucesso");
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao remover permissão: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("GetPermissionsByUserId {UserId}", userId))
//            {
//                try
//                {
//                    return await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        var stopwatch = Stopwatch.StartNew();

//                        _logger.LogInformation("🔍 Buscando permissões do usuário");

//                        var permissions = await _context.UserPermissions
//                            .AsNoTracking()
//                            .Where(up => up.UserId == userId && up.IsActive)
//                            .Select(up => up.Permission)
//                            .ToListAsync(ct);

//                        stopwatch.Stop();
//                        _logger.LogInformation("✅ {Count} permissões encontradas em {Duration}ms",
//                            permissions.Count, stopwatch.ElapsedMilliseconds);

//                        return permissions;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao buscar permissões: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("GetUsersByPermissionId {PermissionId}", permissionId))
//            {
//                try
//                {
//                    return await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        var stopwatch = Stopwatch.StartNew();

//                        _logger.LogInformation("🔍 Buscando usuários com permissão");

//                        var users = await _context.UserPermissions
//                            .AsNoTracking()
//                            .Where(up => up.PermissionId == permissionId && up.IsActive)
//                            .Select(up => up.User)
//                            .Distinct()
//                            .ToListAsync(ct);

//                        stopwatch.Stop();
//                        _logger.LogInformation("✅ {Count} usuários encontrados em {Duration}ms",
//                            users.Count, stopwatch.ElapsedMilliseconds);

//                        return users;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao buscar usuários: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct)
//        {
//            using (_logger.BeginScope("UpdateUserPermissionStatus {UserId} {PermissionId} {Status}", userId, permissionId, status))
//            {
//                try
//                {
//                    await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        _logger.LogInformation("🔄 Atualizando status de permissão para {Status}", status);

//                        var userPermission = await _context.UserPermissions
//                            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

//                        if (userPermission == null)
//                        {
//                            _logger.LogWarning("⚠️ Vínculo não encontrado");
//                            throw new InvalidOperationException("Vínculo não encontrado.");
//                        }

//                        userPermission.IsActive = status;
//                        userPermission.UpdatedAt = DateTime.UtcNow;

//                        _context.UserPermissions.Update(userPermission);
//                        await _context.SaveChangesAsync(ct);

//                        _logger.LogInformation("✅ Status atualizado para {Status}", status);
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao atualizar status: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct)
//        {
//            using (_logger.BeginScope("GetAllUsersWithPermissions"))
//            {
//                try
//                {
//                    return await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        var stopwatch = Stopwatch.StartNew();

//                        _logger.LogInformation("🔍 Buscando todos os usuários com permissões");

//                        var users = await _context.Users
//                            .AsNoTracking()
//                            .Include(u => u.UserPermissions.Where(up => up.IsActive))
//                                .ThenInclude(up => up.Permission)
//                            .Where(u => u.UserPermissions.Any(up => up.IsActive))
//                            .ToListAsync(ct);

//                        stopwatch.Stop();
//                        _logger.LogInformation("✅ {Count} usuários com permissões encontrados em {Duration}ms",
//                            users.Count, stopwatch.ElapsedMilliseconds);

//                        return users;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao buscar usuários com permissões: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("GetUserWithPermissions {UserId}", userId))
//            {
//                try
//                {
//                    return await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        var stopwatch = Stopwatch.StartNew();

//                        _logger.LogInformation("🔍 Buscando usuário com permissões");

//                        var user = await _context.Users
//                            .AsNoTracking()
//                            .Include(u => u.UserPermissions.Where(up => up.IsActive))
//                                .ThenInclude(up => up.Permission)
//                            .FirstOrDefaultAsync(u => u.Id == userId, ct);

//                        stopwatch.Stop();

//                        if (user == null)
//                        {
//                            _logger.LogWarning("⚠️ Usuário não encontrado em {Duration}ms", stopwatch.ElapsedMilliseconds);
//                            return null;
//                        }

//                        _logger.LogInformation("✅ Usuário encontrado com {Count} permissões em {Duration}ms",
//                            user.UserPermissions?.Count ?? 0, stopwatch.ElapsedMilliseconds);

//                        return user;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao buscar usuário: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }

//        public async Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId, CancellationToken ct)
//        {
//            using (_logger.BeginScope("CheckUserPermission {UserId} {PermissionId}", userId, permissionId))
//            {
//                try
//                {
//                    return await _resiliencePolicy.ExecuteAsync(async () =>
//                    {
//                        _logger.LogInformation("🔍 Verificando se usuário possui permissão");

//                        var exists = await _context.UserPermissions
//                            .AsNoTracking()
//                            .AnyAsync(up => up.UserId == userId &&
//                                          up.PermissionId == permissionId &&
//                                          up.IsActive, ct);

//                        if (exists)
//                        {
//                            _logger.LogInformation("✅ Usuário possui permissão");
//                        }
//                        else
//                        {
//                            _logger.LogWarning("⚠️ Usuário não possui permissão");
//                        }

//                        return exists;
//                    });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "❌ Erro ao verificar permissão: {Message}", ex.Message);
//                    throw;
//                }
//            }
//        }
//    }
//}

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
        /// <summary>
        /// Policy de retry APENAS para leituras. Escritas não usam retry
        /// (SaveChangesAsync não é idempotente — retry pode duplicar dados).
        /// </summary>
        private readonly IAsyncPolicy _readPolicy;

        public UserPermissionRepository(
            DefaultContext context,
            ILogger<UserPermissionRepository> logger,
            IAsyncPolicy? readPolicy = null)
            : base(context, logger)
        {
            _readPolicy = readPolicy ?? Policy.NoOpAsync();
        }

        // =========================
        // ESCRITA
        // =========================

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

                    // Idempotência: só atualiza se mudou
                    if (userPermission.IsActive == status)
                        return;

                    userPermission.IsActive = status;
                    userPermission.UpdatedAt = DateTime.UtcNow;

                    // ❌ NÃO precisa de .Update() — já está rastreado
                    await _context.SaveChangesAsync(ct);
                });

        // =========================
        // LEITURA (usa _readPolicy para retry)
        // =========================

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