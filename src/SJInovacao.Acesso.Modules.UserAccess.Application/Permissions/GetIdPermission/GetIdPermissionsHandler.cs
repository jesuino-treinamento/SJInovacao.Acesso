using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.GetIdPermission
{
    public class GetIdPermissionsHandler : IRequestHandler<GetIdPermissionsQuery, PermissionDto>
    {
        private readonly IPermissionRepository _permissionRepository;
        private readonly IMapper _mapper;

        public GetIdPermissionsHandler(IPermissionRepository permissionRepository, IMapper mapper)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
        }

        public async Task<PermissionDto> Handle(GetIdPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);

            return _mapper.Map<PermissionDto>(permission); ;
        }
    }
}
