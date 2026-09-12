using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission
{
    public class DeleteGroupUsersPermissionHandler : IRequestHandler<DeleteGroupUsersPermissionCommand, GroupUsersPermissionDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupUserRepository;
        private readonly IMapper _mapper;
        public DeleteGroupUsersPermissionHandler(IUserRepository userRepository,
            IPermissionRepository permissionRepository, IGroupPermissionRepository groupUserRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupUserRepository = groupUserRepository;
            _mapper = mapper;
        }

        public async Task<GroupUsersPermissionDto> Handle(DeleteGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        {
            // Validação via pipeline (ValidationBehavior)

            var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (existingUser == null)
            {
                throw new DomainException($"User with ID {request.UserId} not found");
            }

            var existingGroup = await _groupUserRepository.GetByIdAsync(request.GroupAccessId, cancellationToken);

            if (existingGroup == null)
                throw new InvalidOperationException($"Grupo {request.GroupAccessId} não existe!");

            var permissionResults = _mapper.Map<List<PermissionDto>>(existingGroup.Permissions ?? new List<Permission>());

            var user = await _userRepository.RemoveGroupFromUserAsync(request.UserId, request.GroupAccessId, cancellationToken);

            var result = new GroupUsersPermissionDto
            {
                UserId = request.UserId,
                UserName = user.Name.ToString(),
                GroupId = request.GroupAccessId,
                GroupName = existingGroup.Name,
                Permissions = permissionResults,
                UserIsActive = request.UserIsActive
            };

            return _mapper.Map<GroupUsersPermissionDto>(result);
        }
    }
}
