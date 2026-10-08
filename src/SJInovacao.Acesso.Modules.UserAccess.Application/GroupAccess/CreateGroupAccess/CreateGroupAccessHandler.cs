using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessHandler : IRequestHandler<CreateGroupAccessCommand, CreateGroupAccessResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        private readonly IMapper _mapper;

        public CreateGroupAccessHandler(IUserRepository userRepository, 
            IPermissionRepository permissionRepository, 
            IGroupPermissionRepository groupPermissionRepository,
            IGroupsPermissionsRepository groupsPermissionsRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupPermissionRepository = groupPermissionRepository;
            _mapper = mapper;
        }
        public async Task<CreateGroupAccessResult> Handle(CreateGroupAccessCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetGroupNameAsync(request.Name, cancellationToken))
                throw new InvalidOperationException($"Grupo {request.Name} já existe!");

            var group = new GroupPermission(request.Name, request.Description);

            if (request.PermissionIds is not null)
            {
                var permissoesFiltradas = (await _permissionRepository.GetAllAsync(cancellationToken))
                    .Where(p => request.PermissionIds.Contains(p.Id) && p.IsActive)
                    .ToList();

                if (permissoesFiltradas.Count == 0)
                    throw new InvalidOperationException($"Não foi encontrada permissão informada!");

                foreach (var permission in permissoesFiltradas)
                {
                    var gp = new GroupsPermissions
                    {
                        GroupId = group.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };


                    gp.Permission = permission;
                    gp.Group = group;
                }
                group.Permissions = permissoesFiltradas;
            }

            await _groupPermissionRepository.CreateAsync(group, cancellationToken);

            return _mapper.Map<CreateGroupAccessResult>(group);
        }
    }
}
