using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.RemoveUserPermission
{
    public class RemoveUserPermissionHandler : IRequestHandler<RemoveUserPermissionCommand, RemoveUserPermissionResult>
    {
        private readonly IUserPermissionRepository _repository;
        private readonly IMapper _mapper;

        public RemoveUserPermissionHandler(IUserPermissionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RemoveUserPermissionResult> Handle(RemoveUserPermissionCommand command, CancellationToken ct)
        {
            await _repository.RemoveAsync(command.UserId, command.PermissionId, ct);

            return new RemoveUserPermissionResult
            {
                UserId = command.UserId,
                PermissionId = command.PermissionId,
                IsActive = false,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
