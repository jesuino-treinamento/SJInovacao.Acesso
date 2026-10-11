using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.UpdateUserPermissionStatus
{
    public class UpdateUserPermissionStatusHandler : IRequestHandler<UpdateUserPermissionStatusCommand, UpdateUserPermissionStatusResult>
    {
        private readonly IUserPermissionRepository _repository;

        public UpdateUserPermissionStatusHandler(IUserPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<UpdateUserPermissionStatusResult> Handle(UpdateUserPermissionStatusCommand command, CancellationToken ct)
        {
            await _repository.UpdateStatusAsync(command.UserId, command.PermissionId, command.IsActive, ct);

            return new UpdateUserPermissionStatusResult
            {
                UserId = command.UserId,
                PermissionId = command.PermissionId,
                IsActive = command.IsActive,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
