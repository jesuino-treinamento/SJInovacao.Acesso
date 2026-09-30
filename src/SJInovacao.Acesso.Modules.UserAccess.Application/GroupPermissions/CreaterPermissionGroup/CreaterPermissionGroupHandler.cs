using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreaterPermissionGroup
{
    public class CreaterPermissionGroupHandler : IRequestHandler<CreaterPermissionGroupCommand, CreaterPermissionGroupResult>
    {
        private readonly IGroupsPermissionsRepository _groupsPermissionsRepository;
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public CreaterPermissionGroupHandler(IGroupsPermissionsRepository groupsPermissionsRepository, 
            IPermissionRepository permissionRepository,
            IGroupPermissionRepository groupPermissionRepository,
            IMapper mapper)
        {
            _groupsPermissionsRepository = groupsPermissionsRepository;
            _permissionRepository = permissionRepository;
            _groupPermissionRepository = groupPermissionRepository;
            _mapper = mapper;
        }

        public async Task<CreaterPermissionGroupResult> Handle(CreaterPermissionGroupCommand command, CancellationToken ct)
        {
            var group = await _groupPermissionRepository.GetByIdAsync(command.GroupId, ct);
            var permission = await _permissionRepository.GetByIdAsync(command.PermissionId, ct);
            if (group == null)
            {
                throw new InvalidOperationException("O grupo não existe!");
            }

            if (permission == null)
            {
                throw new InvalidOperationException("O permissão não existe!");
            }

            //group.Permissions.Add(permission);

            var existgroups = _groupsPermissionsRepository.GetByIdAsync(command.GroupId, command.PermissionId, ct);

            if (existgroups.Result != null)
            {
                throw new InvalidOperationException("Já existe vinculo de grupo para permissão!");
            }

            var groupspermissions = new GroupsPermissions
            {
                Group = group,
                Permission = permission,
                GroupId = command.GroupId,
                PermissionId = command.PermissionId,
                IsActive = true
            };

            await _groupsPermissionsRepository.CreateAsync(groupspermissions, ct);

            return new CreaterPermissionGroupResult
            {
                GroupId = command.GroupId,
                PermissionId = command.PermissionId,
            
                IsActive = true,
                //UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
