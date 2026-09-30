//using AutoMapper;
//using MediatR;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

//namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission
//{
//    public class RemoveGroupPermissionHandler : IRequestHandler<RemoveGroupPermissionCommand, RemoveGroupPermissionResult>
//    {
//        private readonly IGroupPermissionRepository _repository;
//        private readonly IMapper _mapper;

//        public RemoveGroupPermissionHandler(IGroupPermissionRepository repository, IMapper mapper)
//        {
//            _repository = repository;
//            _mapper = mapper;
//        }

//        public async Task<RemoveGroupPermissionResult> Handle(RemoveGroupPermissionCommand command, CancellationToken ct)
//        {
//            await _repository.RemoveAsync(command.GroupId, ct);

//            return new RemoveGroupPermissionResult
//            {
//                GroupId = command.GroupId,
//                PermissionId = command.PermissionId,
//                IsActive = false,
//                UpdatedAt = DateTime.UtcNow
//            };
//        }
//    }
//}

using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using System.Threading;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission
{
    public class RemoveGroupPermissionHandler : IRequestHandler<RemoveGroupPermissionCommand, RemoveGroupPermissionResult>
    {
        private readonly IGroupPermissionRepository _repository;
        private readonly IMapper _mapper;
        private readonly DefaultContext _context;
        private readonly ILogger<RemoveGroupPermissionHandler> _logger;

        public RemoveGroupPermissionHandler(
            IGroupPermissionRepository repository,
            IMapper mapper,
            DefaultContext context,
            ILogger<RemoveGroupPermissionHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _context = context;
            _logger = logger;
        }

        public async Task<RemoveGroupPermissionResult> Handle(RemoveGroupPermissionCommand command, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogInformation("Iniciando remoção da permissão {PermissionId} do grupo {GroupId}", command.PermissionId, command.GroupId);

                var groupAccess = await _repository.GetByIdAsync(command.GroupId, cancellationToken);
                if (groupAccess == null)
                {
                    _logger.LogError("Permissão {PermissionId} não encontrada para o grupo {GroupId}", command.PermissionId, command.GroupId);
                    throw new KeyNotFoundException($"GroupPermission with GroupId {command.GroupId} not found.");
                }

                // Remove GroupPermissions
                var groupPermissions = await _context.GroupsPermissions
                    .Where(gp => gp.GroupId == command.GroupId && gp.PermissionId == command.PermissionId)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Removendo de grupo para GroupId {GroupId}", command.GroupId);
                _context.RemoveRange(groupPermissions);

                //// Remove UsersGroupsPermissions
                //var usersGroupsPermissions = await _context.UsersGroupsPermissions
                //    .Where(ugp => ugp.GroupId == command.GroupId)
                //    .ToListAsync(cancellationToken);

                //_logger.LogInformation("Removendo {Count} permissões de usuários em grupo para GroupId {GroupId}", usersGroupsPermissions.Count, command.GroupId);
                //_context.RemoveRange(usersGroupsPermissions);

                //// Remove UserGroup
                //var userGroups = await _context.UserGroup
                //    .Where(ugp => ugp.GroupId == command.GroupId)
                //    .ToListAsync(cancellationToken);

                //_logger.LogInformation("Removendo {Count} vínculos de usuários ao grupo {GroupId}", userGroups.Count, command.GroupId);
                //_context.RemoveRange(userGroups);

                // Remove o próprio GroupPermission
                //_context.Remove(groupAccess);



                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Permissão {PermissionId} removida com sucesso do grupo {GroupId}", command.PermissionId, command.GroupId);

                //await _repository.RemoveAsync(command.GroupId, cancellationToken);

                return new RemoveGroupPermissionResult
                {
                    GroupId = command.GroupId,
                    PermissionId = command.PermissionId,
                    IsActive = false,
                    UpdatedAt = DateTime.UtcNow
                };
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Tentativa de remover permissão inexistente {PermissionId} do grupo {GroupId}", command.PermissionId, command.GroupId);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Erro inesperado ao remover permissão {PermissionId} do grupo {GroupId}", command.PermissionId, command.GroupId);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}

