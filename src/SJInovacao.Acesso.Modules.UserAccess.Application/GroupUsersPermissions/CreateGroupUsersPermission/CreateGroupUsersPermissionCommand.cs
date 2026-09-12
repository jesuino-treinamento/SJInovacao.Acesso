using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission
{
    public class CreateGroupUsersPermissionCommand : IRequest<GroupUsersPermissionDto>
    {
        public Guid UserId { get; set; }
        public Guid GroupAccessId { get; set; }
        
    }
}
