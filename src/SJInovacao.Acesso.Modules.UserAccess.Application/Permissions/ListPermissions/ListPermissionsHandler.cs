using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions
{
    public class ListPermissionsHandler : IRequestHandler<ListPermissionsQuery, List<PermissionDto>>
    {
        private readonly DefaultContext _context;
        private readonly IMapper _mapper;

        public ListPermissionsHandler(DefaultContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<PermissionDto>> Handle(ListPermissionsQuery request, CancellationToken cancellationToken)
        {
            var permissions = await _context.Permissions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            return _mapper.Map<List<PermissionDto>>(permissions);
        }
    }
}
