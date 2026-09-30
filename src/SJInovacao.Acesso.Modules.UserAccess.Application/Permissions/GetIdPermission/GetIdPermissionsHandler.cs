using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using System.Security;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.GetIdPermission
{
    public class GetIdPermissionsHandler : IRequestHandler<GetIdPermissionsQuery, PermissionDto>
    {
        private readonly DefaultContext _context;
        private readonly IPermissionRepository _permissionRepository;
        private readonly ILogger<GetIdPermissionsHandler> _logger;
        private readonly IMapper _mapper;

        public GetIdPermissionsHandler(DefaultContext context, IPermissionRepository permissionRepository, ILogger<GetIdPermissionsHandler> logger, IMapper mapper)
        {
            _context = context;
            _permissionRepository = permissionRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<PermissionDto> Handle(GetIdPermissionsQuery request,
            CancellationToken cancellationToken)
        {
            var permission = await _permissionRepository.GetByIdAsync(request.PermissionId, cancellationToken);

            //var user = await _context.Users
            //    .Include(u => u.UserPermissions)              // Permissões diretas do usuário
            //        .ThenInclude(up => up.Permission)
            //    .Include(u => u.UserGroups)                       // Grupos do usuário
            //        .ThenInclude(g => g.Group.Permissions)          // Permissões dos grupos
            //    .FirstOrDefaultAsync(u => u.Id == request.PermissionId, cancellationToken);

            //if (user == null)
            //    return new List<PermissionDto>();

            //// Junta permissões diretas e dos grupos, eliminando duplicadas
            //var permissions = user.UserPermissions
            //    .Where(up => up.IsActive)
            //    .Select(up => up.Permission)
            //    .Concat(user.UserGroups.SelectMany(g => g.Group.Permissions))
            //    .Distinct() // evita repetir permissões iguais
            //    .Select(p => new PermissionDto
            //    {
            //        Id = p.Id,
            //        Name = p.Name,
            //        Description = p.Description
            //    })
            //    .ToList();

            // Adiciona permissão extra se o usuário for Admin
            //if (user.Role == UserRole.Admin)
            //{
            //    permissions.Add(new PermissionDto
            //    {
            //        Id = Guid.Empty,
            //        Name = "AdminAccess",
            //        Description = "Full administrative access"
            //    });
            //}

            return _mapper.Map<PermissionDto>(permission); ;
        }


    }
}
