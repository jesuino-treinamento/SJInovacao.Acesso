using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission
{
    public class UpdateGroupUsersPermissionHandler : IRequestHandler<UpdateGroupUsersPermissionCommand, GroupUsersPermissionDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupUserRepository;
        private readonly IUsersGroupsPermissionsRepository _usersGroupsPermissionsRepository;
        private readonly IMapper _mapper;
        public UpdateGroupUsersPermissionHandler(IUserRepository userRepository,
            IPermissionRepository permissionRepository, IGroupPermissionRepository groupUserRepository,
            IUsersGroupsPermissionsRepository usersGroupsPermissionsRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupUserRepository = groupUserRepository;
            _usersGroupsPermissionsRepository = usersGroupsPermissionsRepository;
            _mapper = mapper;
        }

        public async Task<GroupUsersPermissionDto> Handle(UpdateGroupUsersPermissionCommand request, CancellationToken cancellationToken)
        {
            // Validação via pipeline (ValidationBehavior)

            var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (existingUser == null)
            {
                throw new DomainException($"User with ID {request.UserId} not found");
            }

            var existingGroup = await _usersGroupsPermissionsRepository.GetByUserGroupAsync(request.UserId, request.GroupAccessId, cancellationToken);

            if (existingGroup == null)
                throw new InvalidOperationException($"Grupo {request.GroupAccessId} não existe!");            

            // Junta todas as permissões dos vínculos
            //var allPermissions = existingGroup.Select(g => g.Permission).ToList();            

            var permissionsAtualizadas = await _userRepository.UpdateGroupPermissionAsync(request.UserId, request.GroupAccessId, request.UserIsActive, request.PermissionIds, request.PermissionIsActive, cancellationToken);
            var permissionResults = _mapper.Map<List<PermissionDto>>(permissionsAtualizadas ?? new List<Permission>());
            
            var result = new GroupUsersPermissionDto
            {
                UserId = request.UserId,
                UserName = existingUser.Name.ToString(),
                GroupId = request.GroupAccessId,
                GroupName = existingGroup.Select(g => g.Group.Name).FirstOrDefault(),
                Permissions = permissionResults,
                UserIsActive = request.UserIsActive,
                PermissionIsActive = request.PermissionIsActive ?? true
                //UserIsActive = request.PermissionIsActive
            };

            return _mapper.Map<GroupUsersPermissionDto>(result);
        }


        //public async Task<GroupUsersPermissionDto> Handle(UpdateGroupUsersPermissionCommand command, CancellationToken ct)
        //{
        //    // Caso 1: Atualizar status do usuário no grupo
        //    if (command.PermissionId == null)
        //    {
        //        var userGroup = await _context.UserGroups
        //            .FirstOrDefaultAsync(ug => ug.UserId == command.UserId && ug.GroupId == command.GroupAccessId, ct);

        //        if (userGroup == null)
        //            throw new Exception("UserGroup não encontrado");

        //        userGroup.IsActive = command.UserIsActive;
        //        userGroup.UpdatedAt = DateTime.UtcNow;

        //        await _context.SaveChangesAsync(ct);

        //        return new GroupUsersPermissionDto
        //        {
        //            UserId = command.UserId,
        //            GroupId = command.GroupAccessId,
        //            UserIsActive = userGroup.IsActive
        //        };
        //    }

        //    // Caso 2: Atualizar status da permissão no grupo do usuário
        //    else
        //    {
        //        var userGroupPermission = await _context.GroupsPermissions
        //            .FirstOrDefaultAsync(gp => gp.UserId == command.UserId
        //                                    && gp.GroupId == command.GroupAccessId
        //                                    && gp.PermissionId == command.PermissionId, ct);

        //        if (userGroupPermission == null)
        //            throw new Exception("Permissão não encontrada para este usuário e grupo");

        //        userGroupPermission.IsActive = command.PermissionIsActive ?? false;
        //        userGroupPermission.UpdatedAt = DateTime.UtcNow;

        //        await _context.SaveChangesAsync(ct);

        //        return new GroupUsersPermissionDto
        //        {
        //            UserId = command.UserId,
        //            GroupId = command.GroupAccessId,
        //            PermissionId = command.PermissionId,
        //            PermissionIsActive = userGroupPermission.IsActive
        //        };
        //    }
        //}
    }
}
