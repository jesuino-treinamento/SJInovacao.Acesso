using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.AddUserPermission
{
    public class RemoveGroupUserHandler : IRequestHandler<RemoveGroupUserCommand>
    {
        private readonly IUserRepository _userRepository;

        public RemoveGroupUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Unit> Handle(RemoveGroupUserCommand request, CancellationToken cancellationToken)
        {
            await _userRepository.RemoveGroupFromUserAsync(request.UserId, request.GroupUsersId, cancellationToken);
            return Unit.Value;
        }

        Task IRequestHandler<RemoveGroupUserCommand>.Handle(RemoveGroupUserCommand request, CancellationToken cancellationToken)
        {
            //await _userRepository.RemovePermissionFromUserAsync(request.UserId, request.PermissionId, cancellationToken);
            return Handle(request, cancellationToken);
        }
    }
}
