using AutoMapper;
using FluentValidation;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission
{
    public class CreateGroupUsersPermissionHandler : IRequestHandler<CreateGroupUsersPermissionCommand, GroupUsersPermissionDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        public CreateGroupUsersPermissionHandler(IUserRepository userRepository,            
            IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }
        public async Task<GroupUsersPermissionDto> Handle(CreateGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        {
            var validator = new CreateGroupUsersPermissionValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            var user = await _userRepository.AddGroupToUserAsync(request.UserId, request.GroupAccessId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"Usuário {request.UserId} não encontrado.");

            var group = user.UserGroups
                .Select(ug => ug.Group)
                .FirstOrDefault(g => g.Id == request.GroupAccessId);

            if (group == null)
                throw new KeyNotFoundException($"Grupo {request.GroupAccessId} não encontrado.");

            var permissions = user.UsersGroupsPermissions
                .Where(ugp => ugp.GroupId == request.GroupAccessId && ugp.IsActive)
                .Select(ugp => ugp.Permission)
                .ToList();

            var permissionResults = _mapper.Map<List<PermissionDto>>(permissions);

            return new GroupUsersPermissionDto
            {
                UserId = user.Id,
                UserName = user.Name.ToString(),
                GroupId = group.Id,
                GroupName = group.Name,
                Permissions = permissionResults
            };
        }
    }
}
