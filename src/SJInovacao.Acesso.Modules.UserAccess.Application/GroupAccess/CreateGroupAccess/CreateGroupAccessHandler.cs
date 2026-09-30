using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessHandler : IRequestHandler<CreateGroupAccessCommand, CreateGroupAccessResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        private readonly IGroupsPermissionsRepository _groupsPermissionsRepository;
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
            _groupsPermissionsRepository = groupsPermissionsRepository;
            _mapper = mapper;
        }

        public async Task<CreateGroupAccessResult> Handle(CreateGroupAccessCommand request, CancellationToken cancellationToken)
        {
            if (await _userRepository.GetGroupNameAsync(request.Name, cancellationToken))
                throw new InvalidOperationException($"Grupo {request.Name} já existe!");

            var group = new GroupPermission(request.Name, request.Description);

            // Salva o grupo primeiro
            await _groupPermissionRepository.CreateAsync(group, cancellationToken);

            if (request.PermissionIds is not null)
            {
                var permissoesFiltradas = (await _permissionRepository.GetAllAsync(cancellationToken))
                    .Where(p => request.PermissionIds.Contains(p.Id) && p.IsActive)
                    .ToList();

                if (permissoesFiltradas.Count == 0)
                    throw new InvalidOperationException($"Não foi encontrada permissão informada!");

                // Cria vínculos em GroupsPermissions
                foreach (var permission in permissoesFiltradas)
                {
                    var gp = new GroupsPermissions
                    {
                        //UserId = Guid.Empty, // se não usar usuário aqui, pode remover da entidade
                        GroupId = group.Id,
                        PermissionId = permission.Id,
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };


                    gp.Permission = permission;
                    gp.Group = group;

                    await _groupsPermissionsRepository.CreateAsync(gp, cancellationToken);
                }

                group.Permissions = permissoesFiltradas;
            }

            return _mapper.Map<CreateGroupAccessResult>(group);
        }


        //public async Task<CreateGroupPermissionResult> Handle(CreateGroupPermissionCommand request, CancellationToken cancellationToken)
        //{            
        //    if(await _userRepository.GetGroupNameAsync(request.Name, cancellationToken))
        //        throw new InvalidOperationException($"Grupo {request.Name} já existe!");

        //    var group = new GroupPermission(request.Name, request.Description);

        //    if (request.PermissionIds is not null)
        //    {
        //        var permissoesFiltradas = (await _permissionRepository.GetAllAsync(cancellationToken))
        //           .Where(p => request.PermissionIds.Contains(p.Id) && p.IsActive == true)
        //           .ToList();

        //        if(permissoesFiltradas.Count == 0)
        //            throw new InvalidOperationException($"Não foi encontrada permissão informada!");

        //        group.Permissions = permissoesFiltradas;

        //        //foreach (var permission in permissoesFiltradas)
        //        //    group.AddPermission(permission);
        //    }


        //    await _groupUserRepository.CreateAsync(group, cancellationToken);            

        //    return _mapper.Map<CreateGroupPermissionResult>(group);
        //}
    }
}
