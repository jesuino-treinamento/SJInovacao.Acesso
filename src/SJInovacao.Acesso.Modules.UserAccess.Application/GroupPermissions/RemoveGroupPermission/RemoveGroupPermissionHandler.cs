using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.RemoveGroupPermission
{
    public class RemoveGroupPermissionHandler : IRequestHandler<RemoveGroupPermissionCommand, RemoveGroupPermissionResult>
    {
        private readonly IGroupPermissionRepository _repository;
        private readonly IMapper _mapper;

        public RemoveGroupPermissionHandler(IGroupPermissionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RemoveGroupPermissionResult> Handle(RemoveGroupPermissionCommand command, CancellationToken ct)
        {
            await _repository.RemoveAsync(command.GroupId, command.PermissionId, ct);

            return new RemoveGroupPermissionResult
            {
                GroupId = command.GroupId,
                PermissionId = command.PermissionId,
                IsActive = false,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
