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

        //public async Task<GroupUsersPermissionDto> Handle(CreateGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        //{
        //    var validator = new CreateGroupUsersPermissionValidator();
        //    var validationResult = await validator.ValidateAsync(request, cancellationToken);

        //    if (!validationResult.IsValid)
        //        throw new ValidationException(validationResult.Errors);

        //    var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        //    if (existingUser == null)
        //    {
        //        throw new KeyNotFoundException($"User with ID {request.UserId} not found");
        //    }

        //    var existingGroup = await _groupUserRepository.GetByIdAsync(request.GroupAccessId, cancellationToken) ?? new GroupPermission();

        //    if (existingGroup == null)
        //        throw new KeyNotFoundException($"Grupo {request.GroupAccessId} já existe!");

        //    //var permissions = existingGroup.GroupsPermissions.Select(p => p.Permission).ToList();
        //    var permissions = existingGroup.Permissions;
        //    // Mapeia as permissões manualmente
        //    var permissionResults = _mapper.Map<List<PermissionDto?>>(permissions);// ?? new List<Permission>());

        //    var user = await _userRepository.AddGroupToUserAsync(request.UserId, request.GroupAccessId, cancellationToken);

        //    var result = new GroupUsersPermissionDto
        //    {
        //        UserId = request.UserId,
        //        UserName = user.Name.ToString(),
        //        GroupId = request.GroupAccessId,
        //        GroupName = existingGroup.Name,
        //        Permissions = permissionResults
        //    };

        //    return _mapper.Map<GroupUsersPermissionDto>(result);
        //}

        public async Task<GroupUsersPermissionDto> Handle(CreateGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Validação
            var validator = new CreateGroupUsersPermissionValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new ValidationException(validationResult.Errors);

            // 2️⃣ Vincular usuário ao grupo e permissões
            var user = await _userRepository.AddGroupToUserAsync(request.UserId, request.GroupAccessId, cancellationToken);

            if (user == null)
                throw new KeyNotFoundException($"Usuário {request.UserId} não encontrado.");

            // 3️⃣ Obter grupo já carregado no retorno
            var group = user.UserGroups
                .Select(ug => ug.Group)
                .FirstOrDefault(g => g.Id == request.GroupAccessId);

            if (group == null)
                throw new KeyNotFoundException($"Grupo {request.GroupAccessId} não encontrado.");

            // 4️⃣ Obter permissões já carregadas
            var permissions = user.UsersGroupsPermissions
                .Where(ugp => ugp.GroupId == request.GroupAccessId && ugp.IsActive)
                .Select(ugp => ugp.Permission)
                .ToList();

            

            var permissionResults = _mapper.Map<List<PermissionDto>>(permissions);

            // 5️⃣ Montar DTO final
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
