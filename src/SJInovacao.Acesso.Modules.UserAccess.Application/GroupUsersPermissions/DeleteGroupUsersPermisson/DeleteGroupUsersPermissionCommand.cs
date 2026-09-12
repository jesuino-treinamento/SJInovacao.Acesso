using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission
{
    public class DeleteGroupUsersPermissionCommand : IRequest<GroupUsersPermissionDto>
    {
        public Guid UserId { get; set; }
        public Guid GroupAccessId { get; set; }

        public bool UserIsActive { get; set; }

    }
}
