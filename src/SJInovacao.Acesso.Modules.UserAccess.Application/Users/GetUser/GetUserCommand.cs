using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GetUser
{
    public class GetUserCommand : IRequest<UserResult>
    {
        public Guid Id { get; }

        public GetUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
