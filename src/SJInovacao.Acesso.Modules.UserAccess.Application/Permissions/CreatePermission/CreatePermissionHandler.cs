using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission
{
    public class CreatePermissionHandler : IRequestHandler<CreatePermissionCommand, PermissionDto>
    {
       // private readonly DefaultContext _context;
        private readonly IPermissionRepository _permissionRepository;

        private readonly IMapper _mapper;

        public CreatePermissionHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<PermissionDto> Handle(CreatePermissionCommand request, CancellationToken cancellationToken)
        {
            //var permission = new Permission { Name = request.Name,  Description = request.Description };
            //_context.Permissions.Add(permission);
            //await _context.SaveChangesAsync(cancellationToken);
            //return _mapper.Map<PermissionResult>(permission);

            var permission = _mapper.Map<Permission>(request);
            //permission.Activate();
            var createdPermission = await _permissionRepository.CreateAsync(permission, cancellationToken);
            var result = _mapper.Map<PermissionDto>(createdPermission);
            return result;
        }
    }
}
