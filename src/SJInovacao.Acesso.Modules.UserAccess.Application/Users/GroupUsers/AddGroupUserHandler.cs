using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GroupUsers
{
    public class AddGroupUserHandler : IRequestHandler<AddGroupUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public AddGroupUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(AddGroupUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.AddGroupToUserAsync(request.UserId, request.GroupUserId, cancellationToken);
            return Unit.Value;
        }
        //Esta errado  IRequestHandler<AddUserPermissionCommand>
        Task IRequestHandler<AddGroupUserCommand>.Handle(AddGroupUserCommand request, CancellationToken cancellationToken)
        {
            return Handle(request, cancellationToken);
        }
    }
}
