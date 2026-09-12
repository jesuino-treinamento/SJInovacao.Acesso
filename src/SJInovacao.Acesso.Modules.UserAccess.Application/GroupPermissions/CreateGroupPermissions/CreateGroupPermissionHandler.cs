using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionHandler : IRequestHandler<CreateGroupPermissionCommand, CreateGroupPermissionResult>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupUserRepository;
        private readonly IMapper _mapper;

        public CreateGroupPermissionHandler(IUserRepository userRepository, 
            IPermissionRepository permissionRepository, IGroupPermissionRepository groupUserRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _permissionRepository = permissionRepository;
            _groupUserRepository = groupUserRepository;
            _mapper = mapper;
        }

        public async Task<CreateGroupPermissionResult> Handle(CreateGroupPermissionCommand request, CancellationToken cancellationToken)
        {            
            if(await _userRepository.GetGroupNameAsync(request.Name, cancellationToken))
                throw new InvalidOperationException($"Grupo {request.Name} já existe!");

            var group = new GroupPermission(request.Name, request.Description);

            if (request.PermissionIds is not null)
            {
                var permissoesFiltradas = (await _permissionRepository.GetAllAsync(cancellationToken))
                   .Where(p => request.PermissionIds.Contains(p.Id) && p.IsActive == true)
                   .ToList();

                if(permissoesFiltradas.Count == 0)
                    throw new InvalidOperationException($"Não foi encontrada permissão informada!");

                foreach (var permission in permissoesFiltradas)
                    group.AddPermission(permission);
            }


            await _groupUserRepository.CreateAsync(group, cancellationToken);            

            return _mapper.Map<CreateGroupPermissionResult>(group);
        }
    }
}
