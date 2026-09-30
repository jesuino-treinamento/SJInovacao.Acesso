using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission
{
    public class UpdatePermissionHandler : IRequestHandler<UpdatePermissionCommand, PermissionDto>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<UpdatePermissionHandler> _logger;
        private readonly DefaultContext _context;
        private readonly IMapper _mapper;

        public UpdatePermissionHandler(IPermissionRepository permissionRepository, 
            DefaultContext context, ILogger<UpdatePermissionHandler> logger, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _context = context;
            _logger = logger;
            _mapper = mapper;       
        }

        public async Task<PermissionDto> Handle(UpdatePermissionCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                _logger.LogInformation("Iniciando atualização da permissão {PermissionId}", request.Id);

                var validator = new UpdatePermissionValidator();
                var validationResult = await validator.ValidateAsync(request, cancellationToken);

                if (!validationResult.IsValid)
                {
                    _logger.LogWarning("Validação falhou para permissão {PermissionId}: {Errors}",
                        request.Id, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                    throw new ValidationException(validationResult.Errors);
                }

                var permission1 = await _permissionRepository.GetByIdAsync(request.Id, cancellationToken);
                if (permission1 == null)
                {
                    _logger.LogError("Permissão {PermissionId} não encontrada", request.Id);
                    throw new DomainException($"Permission with ID {request.Id} not found for update");
                }

                permission1.Name = request.Name;
                permission1.Description = request.Description;
                permission1.IsActive = request.IsActive;
                permission1.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Atualizando status em cascata para permissão {PermissionId}", request.Id);

                var groupsPermissions = _context.GroupsPermissions.Where(gp => gp.PermissionId == permission1.Id);
                await groupsPermissions.ForEachAsync(gp =>
                {
                    gp.IsActive = permission1.IsActive;
                    gp.UpdatedAt = DateTime.UtcNow;
                }, cancellationToken);

                var entities = await _context.UsersGroupsPermissions
                .Where(ugp => ugp.PermissionId == permission1.Id)
                .ToListAsync(cancellationToken);
                foreach (var entity in entities)
                {
                    entity.IsActive = permission1.IsActive;
                    entity.UpdatedAt = DateTime.UtcNow;
                }

                var usersPermissions = await _context.UserPermissions.Where(up => up.PermissionId == permission1.Id).ToListAsync(cancellationToken);
                foreach (var userPermission in usersPermissions)
                {
                    userPermission.IsActive = permission1.IsActive;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                }

                var permission = _mapper.Map<Permission>(permission1);
                var createdPermission =  _context.Permissions.Update(permission);

                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Permissão {PermissionId} atualizada com sucesso", request.Id);

                return _mapper.Map<PermissionDto>(createdPermission.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar permissão {PermissionId}", request.Id);
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

    }
}
