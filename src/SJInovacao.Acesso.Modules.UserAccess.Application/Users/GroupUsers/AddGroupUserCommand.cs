using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GroupUsers
{
    public class AddGroupUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        //public Guid PermissionId { get; set; }
        public Guid GroupUserId { get; internal set; }
    }
}
