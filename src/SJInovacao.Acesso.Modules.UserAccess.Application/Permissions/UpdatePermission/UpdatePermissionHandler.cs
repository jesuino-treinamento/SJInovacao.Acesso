using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
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

                // Verifica duplicidade de nome
                var allPermissions = await _permissionRepository.GetAllAsync(cancellationToken);
                if (allPermissions.Any(p => p.Id != request.Id && p.Name == request.Name))
                    throw new DomainException($"Já existe uma permissão com o nome '{request.Name}'.");

                // Busca a permissão existente
                var permission = allPermissions.FirstOrDefault(p => p.Id == request.Id);
                if (permission == null)
                {
                    _logger.LogError("Permissão {PermissionId} não encontrada", request.Id);
                    throw new DomainException($"Permission with ID {request.Id} not found for update");
                }

                // Atualiza dados básicos
                permission.Name = request.Name;
                permission.Description = request.Description;
                permission.IsActive = request.IsActive;
                permission.UpdatedAt = DateTime.UtcNow;

                _logger.LogInformation("Atualizando status em cascata para permissão {PermissionId}", request.Id);

                // Atualiza vínculos em GroupsPermissions
                var groupsPermissions = await _context.GroupsPermissions
                    .Where(gp => gp.PermissionId == permission.Id)
                    .ToListAsync(cancellationToken);

                foreach (var gp in groupsPermissions)
                {
                    gp.IsActive = permission.IsActive;
                    gp.UpdatedAt = DateTime.UtcNow;
                }

                // Atualiza vínculos em UsersGroupsPermissions
                var entities = await _context.UsersGroupsPermissions
                    .Where(ugp => ugp.PermissionId == permission.Id)
                    .ToListAsync(cancellationToken);

                foreach (var entity in entities)
                {
                    entity.IsActive = permission.IsActive;
                    entity.UpdatedAt = DateTime.UtcNow;
                }

                // Atualiza vínculos em UserPermissions
                var usersPermissions = await _context.UserPermissions
                    .Where(up => up.PermissionId == permission.Id)
                    .ToListAsync(cancellationToken);

                foreach (var userPermission in usersPermissions)
                {
                    userPermission.IsActive = permission.IsActive;
                    userPermission.UpdatedAt = DateTime.UtcNow;
                }

                // Atualiza a própria permissão
                var updatedPermission = _context.Permissions.Update(permission);

                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                _logger.LogInformation("Permissão {PermissionId} atualizada com sucesso", request.Id);

                return _mapper.Map<PermissionDto>(updatedPermission.Entity);
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
