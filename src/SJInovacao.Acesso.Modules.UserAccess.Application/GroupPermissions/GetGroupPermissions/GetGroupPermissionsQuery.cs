using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetGroupPermissions
{
    public class GetGroupPermissionsQuery : IRequest<IEnumerable<PermissionDto>>
    {
        public Guid GroupId { get; set; }
    }
}

