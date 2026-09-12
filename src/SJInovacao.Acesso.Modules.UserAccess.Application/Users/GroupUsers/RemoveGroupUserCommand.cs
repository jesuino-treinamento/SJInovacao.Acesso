using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.AddUserPermission
{
    public class RemoveGroupUserCommand : IRequest
    {
        public Guid UserId { get; set; }
        public Guid GroupUsersId { get; set; }
    }
}
