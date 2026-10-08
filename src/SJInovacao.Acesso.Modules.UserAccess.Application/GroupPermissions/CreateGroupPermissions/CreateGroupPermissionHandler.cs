using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionHandler : IRequestHandler<CreateGroupPermissionCommand, CreateGroupPermissionResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        //private readonly IGroupsPermissionsRepository _groupsPermissionsRepository;
        private readonly DefaultContext _context;
        private readonly ILogger<CreateGroupPermissionHandler> _logger;
        private readonly IMapper _mapper;

        public CreateGroupPermissionHandler(
            IUserRepository userRepository, 
            IPermissionRepository permissionRepository, 
            IGroupPermissionRepository groupPermissionRepository,
            //IGroupsPermissionsRepository groupsPermissionsRepository,
            DefaultContext context,
            ILogger<CreateGroupPermissionHandler> logger,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupPermissionRepository = groupPermissionRepository;
            //_groupsPermissionsRepository = groupsPermissionsRepository;
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }

        //public async Task<CreateGroupPermissionResult> Handle(CreateGroupPermissionCommand request, CancellationToken cancellationToken)
        //{
        //    if (await _userRepository.GetGroupNameAsync(request.Name, cancellationToken))
        //        throw new InvalidOperationException($"Grupo {request.Name} já existe!");

        //    var group = new GroupPermission(request.Name, request.Description);

        //    // Salva o grupo primeiro
        //    await _groupPermissionRepository.CreateAsync(group, cancellationToken);

        //    if (request.PermissionIds is not null)
        //    {
        //        var permissoesFiltradas = (await _permissionRepository.GetAllAsync(cancellationToken))
        //            .Where(p => request.PermissionIds.Contains(p.Id) && p.IsActive)
        //            .ToList();

        //        if (permissoesFiltradas.Count == 0)
        //            throw new InvalidOperationException($"Não foi encontrada permissão informada!");

        //        // Cria vínculos em GroupsPermissions
        //        //foreach (var permission in permissoesFiltradas)
        //        //{
        //        //    var gp = new GroupsPermissions
        //        //    {
        //        //        //UserId = Guid.Empty, // se não usar usuário aqui, pode remover da entidade
        //        //        GroupId = group.Id,
        //        //        PermissionId = permission.Id,
        //        //        IsActive = true,
        //        //        CreatedAt = group.CreatedAt
        //        //    };


        //        //    gp.Permission = permission;
        //        //    gp.Group = group;

        //        //    //await _groupsPermissionsRepository.CreateAsync(gp, cancellationToken);
        //        //}

        //        group.Permissions = permissoesFiltradas;
        //    }

        //    return _mapper.Map<CreateGroupPermissionResult>(group);
        //}


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

        public async Task<CreateGroupPermissionResult> Handle(CreateGroupPermissionCommand request, CancellationToken cancellationToken)
        {
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
            group = await _groupPermissionRepository.CreateAsync(group, cancellationToken);

            _logger.LogInformation("Grupo {GroupName} criado com sucesso", request.Name);

            return _mapper.Map<CreateGroupPermissionResult>(group);
        }


    }
}
