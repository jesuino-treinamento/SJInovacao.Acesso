using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories
{
    public class GroupsPermissionsRepository : BaseRepository<GroupsPermissions>, IGroupsPermissionsRepository
    {
        public GroupsPermissionsRepository(DefaultContext context, ILogger<GroupsPermissionsRepository> logger)
        : base(context, logger)
        {  }

        //Analise está com icoerente ???
        public async Task<GroupsPermissions?> GetByIdAsync(Guid groupId, Guid permissionId, CancellationToken ct)
            => await _context.GroupsPermissions.FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);

        public async Task<List<GroupsPermissions>> GetAllAsync(CancellationToken ct)
            => await _context.GroupsPermissions.ToListAsync(ct);

        public async Task<GroupsPermissions> CreateAsync(GroupsPermissions entity, CancellationToken ct)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var groupExists = await _context.GroupPermissions
                    .AsNoTracking()
                    .AnyAsync(g => g.Id == entity.GroupId, ct);

                var permissionExists = await _context.Permissions
                    .AsNoTracking()
                    .AnyAsync(p => p.Id == entity.PermissionId, ct);

                if (!groupExists)
                    throw new InvalidOperationException($"Grupo {entity.GroupId} não encontrado.");
                if (!permissionExists)
                    throw new InvalidOperationException($"Permissão {entity.PermissionId} não encontrada.");

                entity.Group = null;
                entity.Permission = null;

                // 3️⃣ Adiciona o vínculo
                await _context.GroupsPermissions.AddAsync(entity, ct);

                // 4️⃣ Salva alterações
                await _context.SaveChangesAsync(ct);

                // 5️⃣ Commit da transação
                await transaction.CommitAsync(ct);

                return entity;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(ct);
                throw new Exception($"Erro ao criar vínculo grupo-permissão: {ex.Message}", ex);
            }
        }



        public async Task<GroupsPermissions> UpdateAsync(GroupsPermissions entity, CancellationToken ct)
        {
            _context.GroupsPermissions.Update(entity);
            await _context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task<bool> DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct)
        {
            // 1️⃣ Buscar o vínculo específico
            var entity = await _context.GroupsPermissions
                .FirstOrDefaultAsync(gp => gp.GroupId == groupId && gp.PermissionId == permissionId, ct);

            if (entity == null)
                return false;

            // 2️⃣ Remover apenas o vínculo
            _context.GroupsPermissions.Remove(entity);

            // 3️⃣ Salvar alterações
            await _context.SaveChangesAsync(ct);

            return true;
        }

        public async Task<IEnumerable<GroupsPermissions>> GetByGroupIdAsync(Guid groupId, CancellationToken ct)
            => await _context.GroupsPermissions.Where(gp => gp.GroupId == groupId).ToListAsync(ct);

        public async Task<IEnumerable<GroupsPermissions>> GetByPermissionIdAsync(Guid permissionId, CancellationToken ct)
            => await _context.GroupsPermissions.Where(gp => gp.PermissionId == permissionId).ToListAsync(ct);

        public async Task UpdateStatusAsync(Guid groupId, Guid permissionId, bool isActive, CancellationToken ct)
        {
            await ExecuteWithLoggingAsync(
                "UpdateStatus",
                async () =>
                {
                    using var transaction = await _context.Database.BeginTransactionAsync(ct);
                    try
                    {
                        var groupPermissions = await _context.GroupsPermissions
                        .Where(gp => gp.GroupId == groupId && gp.PermissionId == permissionId)
                        .ToListAsync(ct) ;

                        if (!groupPermissions.Any())
                        {
                            _logger.LogError("Permissão {PermissionId} não encontrada para o grupo {GroupId}", permissionId, groupId);
                            throw new KeyNotFoundException($"Permissão {permissionId} não encontrada para o grupo {groupId}");
                        }
                        else
                        {
                            _logger.LogInformation("Atualizando de grupo para GroupId {GroupId} as {GroupPermissions}", groupId, groupPermissions.Count());

                            await _context.GroupsPermissions
                                .Where(gp => gp.GroupId == groupId && gp.PermissionId == permissionId)
                                .ForEachAsync(gp =>
                                {
                                    gp.IsActive = isActive;
                                    gp.UpdatedAt = DateTime.UtcNow;
                                }, ct);
                        }
                        await _context.UsersGroupsPermissions
                            .Where(ugp => ugp.GroupId == groupId && ugp.PermissionId == permissionId)
                            .ForEachAsync(gp =>
                            {
                                gp.IsActive = isActive;
                                gp.UpdatedAt = DateTime.UtcNow;
                            }, ct);

                        await _context.UserPermissions
                            .Where(ugp => ugp.PermissionId == permissionId)
                            .ForEachAsync(gp =>
                            {
                                gp.IsActive = isActive;
                                gp.UpdatedAt = DateTime.UtcNow;
                            }, ct);

                        _logger.LogInformation("Atualizando vínculos de usuários com permissião {PermissionId}", permissionId);

                        await _context.SaveChangesAsync(ct);
                        await transaction.CommitAsync(ct);

                        _logger.LogInformation("Permissão {PermissionId} atualizada com sucesso do grupo {GroupId}", permissionId, groupId);

                    }
                    catch (KeyNotFoundException ex)
                    {
                        _logger.LogWarning(ex, "Tentativa de atualizar permissão inexistente {PermissionId} do grupo {GroupId}", permissionId, groupId);
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogCritical(ex, "Erro inesperado ao atualizar permissão {PermissionId} do grupo {GroupId}", permissionId, groupId);
                        await transaction.RollbackAsync(ct);
                        throw;
                    }
                });
        }

        public async Task UpdateAllGroupsPermissionStatusAsync(Guid permissionId, bool isActive, CancellationToken cancellationToken)
        {
            var groupsPermissions = _context.GroupsPermissions.Where(gp => gp.PermissionId == permissionId);
            await groupsPermissions.ForEachAsync(gp =>
            {
                gp.IsActive = isActive;
                gp.UpdatedAt = DateTime.UtcNow;
            }, cancellationToken);
           // await _context.SaveChangesAsync(cancellationToken);
        }
    }

}
