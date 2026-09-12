using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions
{
    public class ListPermissionsQuery : IRequest<List<PermissionDto>>
    {
    }
}
