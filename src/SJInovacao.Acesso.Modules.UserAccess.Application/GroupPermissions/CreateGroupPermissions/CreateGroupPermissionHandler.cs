using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionHandler : IRequestHandler<CreateGroupPermissionCommand, CreateGroupPermissionResult>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupPermissionRepository _groupPermissionRepository;
        private readonly DefaultContext _context;
        private readonly ILogger<CreateGroupPermissionHandler> _logger;
        private readonly IMapper _mapper;

        public CreateGroupPermissionHandler(
            IPermissionRepository permissionRepository, 
            IGroupPermissionRepository groupPermissionRepository,
            DefaultContext context,
            ILogger<CreateGroupPermissionHandler> logger,
            IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _groupPermissionRepository = groupPermissionRepository;
            _logger = logger;
            _context = context;
            _mapper = mapper;
        }
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
