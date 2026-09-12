using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.DeleteUser
{
    public record DeleteUserCommand : IRequest<DeleteUserResponse>
    {
        public Guid Id { get; }

        public DeleteUserCommand(Guid id)
        {
            Id = id;
        }
    }
}
