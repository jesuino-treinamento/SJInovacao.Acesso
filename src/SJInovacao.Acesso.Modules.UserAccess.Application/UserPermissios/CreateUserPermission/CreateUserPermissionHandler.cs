using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissios.CreateUserPermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using System.Threading;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.CreateUserPermission
{
    public class CreateUserPermissionHandler : IRequestHandler<CreateUserPermissionCommand, CreateUserPermissionResult>
    {
        private readonly IUserPermissionRepository _UserPermissionrepository;
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public CreateUserPermissionHandler(IUserPermissionRepository userPermissionrepository, IUserRepository userRepository, 
            IPermissionRepository permissionRepository, IMapper mapper)
        {
            _UserPermissionrepository = userPermissionrepository;
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<CreateUserPermissionResult> Handle(CreateUserPermissionCommand command, CancellationToken ct)
        {
            // 1. Validar comando
            var validator = new CreateUserPermissionValidator();
            var validationResult = await validator.ValidateAsync(command, ct);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.GetByIdAsync(command.UserId, ct);
            if (user == null)
                throw new KeyNotFoundException($"Usuário com ID {command.UserId} não encontrado.");

            var exist = await _permissionRepository.GetByIdAsync(command.PermissionId, ct);

            if (exist == null)
            {
                throw new KeyNotFoundException($"Permissão com ID {command.PermissionId} não encontrada.");
            }

            if (await _UserPermissionrepository.GetExistUserWithUsersPermissionsAsync(command.UserId, command.PermissionId, ct))
                    throw new KeyNotFoundException("O vínculo entre usuário e permissão já existe.");

            // 2. Criar vínculo entre usuário e permissão
            await _UserPermissionrepository.AddAsync(command.UserId, command.PermissionId, ct);

            // 3. Mapear resultado
            var userPermission = new UserPermission
            {
                UserId = command.UserId,
                PermissionId = command.PermissionId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            var result = _mapper.Map<CreateUserPermissionResult>(userPermission);
            return result;
        }
    }
}
