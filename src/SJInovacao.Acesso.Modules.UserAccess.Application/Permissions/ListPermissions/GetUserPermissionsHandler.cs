using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions
{
    public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsQuery, List<PermissionDto>>
    {
        private readonly DefaultContext _context;

        public GetUserPermissionsHandler(DefaultContext context)
        {
            _context = context;
        }

        public async Task<List<PermissionDto>> Handle(GetUserPermissionsQuery request, CancellationToken cancellationToken)
        {
            var user = await _context.Users
                .Include(u => u.UserPermissions)              // Permissões diretas do usuário
                    .ThenInclude(up => up.Permission)
                .Include(u => u.UserGroups)                       // Grupos do usuário
                    .ThenInclude(g => g.Group.Permissions)          // Permissões dos grupos
                .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken);

            if (user == null)
                return new List<PermissionDto>();

            // Junta permissões diretas e dos grupos, eliminando duplicadas
            var permissions = user.UserPermissions
                .Where(up => up.IsActive)
                .Select(up => up.Permission)
                .Concat(user.UserGroups.SelectMany(g => g.Group.Permissions))
                .Distinct() // evita repetir permissões iguais
                .Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description
                })
                .ToList();

            // Adiciona permissão extra se o usuário for Admin
            if (user.Role == UserRole.Admin)
            {
                permissions.Add(new PermissionDto
                {
                    Id = Guid.Empty,
                    Name = "AdminAccess",
                    Description = "Full administrative access"
                });
            }

            return permissions;
        }


    }
}
