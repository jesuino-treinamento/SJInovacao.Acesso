using AutoMapper;
using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission
{
    public class CreateGroupUsersPermissionHandler : IRequestHandler<CreateGroupUsersPermissionCommand, GroupUsersPermissionDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupUserRepository;
        private readonly IMapper _mapper;
        public CreateGroupUsersPermissionHandler(IUserRepository userRepository,
            IPermissionRepository permissionRepository, IGroupPermissionRepository groupUserRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupUserRepository = groupUserRepository;
            _mapper = mapper;
        }

        public async Task<GroupUsersPermissionDto> Handle(CreateGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateGroupUsersPermissionValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (existingUser == null)
            {
                throw new KeyNotFoundException($"User with ID {request.UserId} not found");
            }

            var existingGroup = await _groupUserRepository.GetByIdAsync(request.GroupAccessId, cancellationToken) ?? new GroupPermission();

            if (existingGroup == null)
                throw new KeyNotFoundException($"Grupo {request.GroupAccessId} já existe!");

            // Mapeia as permissões manualmente
            var permissionResults = _mapper.Map<List<PermissionDto>>(existingGroup?.Permissions ?? new List<Permission>());

            var user = await _userRepository.AddGroupToUserAsync(request.UserId, request.GroupAccessId, cancellationToken);

            var result = new GroupUsersPermissionDto
            {
                UserId = request.UserId,
                UserName = user.Name.ToString(),
                GroupId = request.GroupAccessId,
                GroupName = existingGroup.Name,
                Permissions = permissionResults
            };

            return _mapper.Map<GroupUsersPermissionDto>(result);
        }
    }
}
