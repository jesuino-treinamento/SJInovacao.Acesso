using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using System.Threading;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.DeleteGroupAccess
{
    public class DeleteGroupAccessHandler : IRequestHandler<DeleteGroupAccessCommand, DeleteGroupAccessResult>
    {
        private readonly IGroupPermissionRepository _repository;
        private readonly ILogger<DeleteGroupAccessHandler> _logger;
        private readonly DefaultContext _context;
        private readonly IMapper _mapper;

        public DeleteGroupAccessHandler(IGroupPermissionRepository repository, IMapper mapper, DefaultContext context, ILogger<DeleteGroupAccessHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public async Task<DeleteGroupAccessResult> Handle(DeleteGroupAccessCommand command, CancellationToken ct)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(ct);

            try
            {
                var groupAccess = await _repository.GetByIdAsync(command.GroupId, ct);
                if (groupAccess == null)
                {
                    _logger.LogError("Grupo {GroupId} não encontrado para exclusão", command.GroupId);
                    throw new KeyNotFoundException($"Group with ID {command.GroupId} not found.");
                }

                _logger.LogInformation("Iniciando exclusão em cascata para grupo {GroupId}", command.GroupId);

                var groupsPermissions = await _context.GroupsPermissions
                    .Where(gp => gp.GroupId == command.GroupId)
                    .ToListAsync(ct);

                _logger.LogInformation("Removendo {Count} permissões de grupo para GroupId {GroupId}", groupsPermissions.Count, command.GroupId);
                _context.RemoveRange(groupsPermissions);

                var usersGroupsPermissions = await _context.UsersGroupsPermissions
                    .Where(ugp => ugp.GroupId == command.GroupId)
                    .ToListAsync(ct);

                _logger.LogInformation("Removendo {Count} permissões de usuários em grupo para GroupId {GroupId}", usersGroupsPermissions.Count, command.GroupId);
                _context.RemoveRange(usersGroupsPermissions);

                var userGroups = await _context.UserGroup
                    .Where(ugp => ugp.GroupId == command.GroupId)
                    .ToListAsync(ct);

                _logger.LogInformation("Removendo {Count} vínculos de usuários ao grupo {GroupId}", userGroups.Count, command.GroupId);
                _context.RemoveRange(userGroups);

                var groupPermissions = await _context.GroupPermissions
                    .Where(gp => gp.Id == command.GroupId)
                    .ToListAsync(ct);

                _logger.LogInformation("Removendo {Count} permissões de grupo para GroupId {GroupId}", groupsPermissions.Count, command.GroupId);
                _context.RemoveRange(groupPermissions);               

                _logger.LogInformation("Excluindo grupo {GroupId} - {GroupName}", command.GroupId, groupAccess.Name);

                await _context.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);

                _logger.LogInformation("Grupo {GroupId} excluído com sucesso", command.GroupId);

                return new DeleteGroupAccessResult
                {
                    GroupId = command.GroupId,
                    Name = groupAccess.Name
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tentativa de excluir grupo inexistente {GroupId}", command.GroupId);
                await transaction.RollbackAsync(ct);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado ao excluir grupo {GroupId}", command.GroupId);
                await transaction.RollbackAsync(ct);
                throw;
            }
        }
    }
}
