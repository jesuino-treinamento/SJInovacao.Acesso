using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using System.Diagnostics;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    /// <summary>
    /// Repositório para gerenciar permissões de usuário com logging e resiliência.
    /// </summary>
    public class UserPermissionRepository : IUserPermissionRepository
    {
        private readonly DefaultContext _context;
        private readonly ILogger<UserPermissionRepository> _logger;
        private readonly IAsyncPolicy _resiliencePolicy;

        public UserPermissionRepository(
            DefaultContext context,
            ILogger<UserPermissionRepository> logger,
            IAsyncPolicy? resiliencePolicy = null)
        {
            _context = context;
            _logger = logger;
            _resiliencePolicy = resiliencePolicy ?? Policy.NoOpAsync();
        }

        /// <summary>
        /// Adiciona um vínculo entre usuário e permissão
        /// </summary>
        public async Task AddAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            using (_logger.BeginScope("AddUserPermission {UserId} {PermissionId}", userId, permissionId))
            {
                try
                {
                    await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation("➕ Adicionando permissão para usuário {UserId}", userId);

                        // Validar se ambos existem
                        var userExists = await _context.Users
                            .AnyAsync(u => u.Id == userId, ct);

                        if (!userExists)
                        {
                            _logger.LogWarning("⚠️ Usuário {UserId} não encontrado", userId);
                            throw new InvalidOperationException($"Usuário com ID {userId} não existe.");
                        }

                        var permissionExists = await _context.Permissions
                            .AnyAsync(p => p.Id == permissionId, ct);

                        if (!permissionExists)
                        {
                            _logger.LogWarning("⚠️ Permissão {PermissionId} não encontrada", permissionId);
                            throw new InvalidOperationException($"Permissão com ID {permissionId} não existe.");
                        }

                        // Verificar se o vínculo já existe
                        var alreadyExists = await _context.UserPermissions
                            .AnyAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

                        if (alreadyExists)
                        {
                            _logger.LogWarning("⚠️ Vínculo já existe para usuário {UserId} e permissão {PermissionId}", userId, permissionId);
                            throw new InvalidOperationException("Este vínculo já existe.");
                        }

                        var userPermission = new UserPermission
                        {
                            UserId = userId,
                            PermissionId = permissionId,
                            IsActive = true,
                            CreatedAt = DateTime.UtcNow
                        };

                        _context.UserPermissions.Add(userPermission);
                        await _context.SaveChangesAsync(ct);

                        _logger.LogInformation("✅ Permissão adicionada com sucesso");
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao adicionar permissão: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Remove o vínculo entre usuário e permissão
        /// </summary>
        public async Task RemoveAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            using (_logger.BeginScope("RemoveUserPermission {UserId} {PermissionId}", userId, permissionId))
            {
                try
                {
                    await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation("🗑️ Removendo permissão de usuário");

                        var userPermission = await _context.UserPermissions
                            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

                        if (userPermission == null)
                        {
                            _logger.LogWarning("⚠️ Vínculo não encontrado");
                            throw new InvalidOperationException("Vínculo não encontrado.");
                        }

                        _context.UserPermissions.Remove(userPermission);
                        await _context.SaveChangesAsync(ct);

                        _logger.LogInformation("✅ Permissão removida com sucesso");
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao remover permissão: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna todas as permissões de um usuário
        /// </summary>
        public async Task<IEnumerable<Permission>> GetByUserIdAsync(Guid userId, CancellationToken ct)
        {
            using (_logger.BeginScope("GetPermissionsByUserId {UserId}", userId))
            {
                try
                {
                    return await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        var stopwatch = Stopwatch.StartNew();

                        _logger.LogInformation("🔍 Buscando permissões do usuário");

                        var permissions = await _context.UserPermissions
                            .AsNoTracking()
                            .Where(up => up.UserId == userId && up.IsActive)
                            .Select(up => up.Permission)
                            .ToListAsync(ct);

                        stopwatch.Stop();
                        _logger.LogInformation("✅ {Count} permissões encontradas em {Duration}ms",
                            permissions.Count, stopwatch.ElapsedMilliseconds);

                        return permissions;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao buscar permissões: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna todos os usuários que possuem uma permissão específica
        /// </summary>
        public async Task<IEnumerable<User>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
        {
            using (_logger.BeginScope("GetUsersByPermissionId {PermissionId}", permissionId))
            {
                try
                {
                    return await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        var stopwatch = Stopwatch.StartNew();

                        _logger.LogInformation("🔍 Buscando usuários com permissão");

                        var users = await _context.UserPermissions
                            .AsNoTracking()
                            .Where(up => up.PermissionId == permissionId && up.IsActive)
                            .Select(up => up.User)
                            .Distinct()
                            .ToListAsync(ct);

                        stopwatch.Stop();
                        _logger.LogInformation("✅ {Count} usuários encontrados em {Duration}ms",
                            users.Count, stopwatch.ElapsedMilliseconds);

                        return users;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao buscar usuários: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Altera o status (ativo/inativo) de uma permissão de usuário
        /// </summary>
        public async Task UpdateStatusAsync(Guid userId, Guid permissionId, bool status, CancellationToken ct)
        {
            using (_logger.BeginScope("UpdateUserPermissionStatus {UserId} {PermissionId} {Status}", userId, permissionId, status))
            {
                try
                {
                    await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation("🔄 Atualizando status de permissão para {Status}", status);

                        var userPermission = await _context.UserPermissions
                            .FirstOrDefaultAsync(up => up.UserId == userId && up.PermissionId == permissionId, ct);

                        if (userPermission == null)
                        {
                            _logger.LogWarning("⚠️ Vínculo não encontrado");
                            throw new InvalidOperationException("Vínculo não encontrado.");
                        }

                        userPermission.IsActive = status;
                        userPermission.UpdatedAt = DateTime.UtcNow;

                        _context.UserPermissions.Update(userPermission);
                        await _context.SaveChangesAsync(ct);

                        _logger.LogInformation("✅ Status atualizado para {Status}", status);
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao atualizar status: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna todos os usuários com suas permissões carregadas
        /// </summary>
        public async Task<IEnumerable<User>> GetAllWithPermissionsAsync(CancellationToken ct)
        {
            using (_logger.BeginScope("GetAllUsersWithPermissions"))
            {
                try
                {
                    return await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        var stopwatch = Stopwatch.StartNew();

                        _logger.LogInformation("🔍 Buscando todos os usuários com permissões");

                        var users = await _context.Users
                            .AsNoTracking()
                            .Include(u => u.UserPermissions.Where(up => up.IsActive))
                                .ThenInclude(up => up.Permission)
                            .Where(u => u.UserPermissions.Any(up => up.IsActive))
                            .ToListAsync(ct);

                        stopwatch.Stop();
                        _logger.LogInformation("✅ {Count} usuários com permissões encontrados em {Duration}ms",
                            users.Count, stopwatch.ElapsedMilliseconds);

                        return users;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao buscar usuários com permissões: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Retorna um usuário específico com suas permissões carregadas
        /// </summary>
        public async Task<User?> GetUserWithPermissionsAsync(Guid userId, CancellationToken ct)
        {
            using (_logger.BeginScope("GetUserWithPermissions {UserId}", userId))
            {
                try
                {
                    return await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        var stopwatch = Stopwatch.StartNew();

                        _logger.LogInformation("🔍 Buscando usuário com permissões");

                        var user = await _context.Users
                            .AsNoTracking()
                            .Include(u => u.UserPermissions.Where(up => up.IsActive))
                                .ThenInclude(up => up.Permission)
                            .FirstOrDefaultAsync(u => u.Id == userId, ct);

                        stopwatch.Stop();

                        if (user == null)
                        {
                            _logger.LogWarning("⚠️ Usuário não encontrado em {Duration}ms", stopwatch.ElapsedMilliseconds);
                            return null;
                        }

                        _logger.LogInformation("✅ Usuário encontrado com {Count} permissões em {Duration}ms",
                            user.UserPermissions?.Count ?? 0, stopwatch.ElapsedMilliseconds);

                        return user;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao buscar usuário: {Message}", ex.Message);
                    throw;
                }
            }
        }

        /// <summary>
        /// Verifica se um usuário possui uma permissão específica
        /// </summary>
        public async Task<bool> GetExistUserWithUsersPermissionsAsync(Guid userId, Guid permissionId, CancellationToken ct)
        {
            using (_logger.BeginScope("CheckUserPermission {UserId} {PermissionId}", userId, permissionId))
            {
                try
                {
                    return await _resiliencePolicy.ExecuteAsync(async () =>
                    {
                        _logger.LogInformation("🔍 Verificando se usuário possui permissão");

                        var exists = await _context.UserPermissions
                            .AsNoTracking()
                            .AnyAsync(up => up.UserId == userId &&
                                          up.PermissionId == permissionId &&
                                          up.IsActive, ct);

                        if (exists)
                        {
                            _logger.LogInformation("✅ Usuário possui permissão");
                        }
                        else
                        {
                            _logger.LogWarning("⚠️ Usuário não possui permissão");
                        }

                        return exists;
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "❌ Erro ao verificar permissão: {Message}", ex.Message);
                    throw;
                }
            }
        }
    }
}