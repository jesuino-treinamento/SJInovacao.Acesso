using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.GroupPermissions.RemoveGroupPermission.UpdateGroupPermissionStatus
{
    public class UpdateGroupPermissionStatusHandler : IRequestHandler<UpdateGroupPermissionStatusCommand, UpdateGroupPermissionStatusResult>
    {
        private readonly IGroupPermissionRepository _repository;
        private readonly IMapper _mapper;

        public UpdateGroupPermissionStatusHandler(IGroupPermissionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UpdateGroupPermissionStatusResult> Handle(UpdateGroupPermissionStatusCommand command, CancellationToken ct)
        {
            //var validator = new UpdateUserPermissionStatusValidator();
            //var validationResult = await validator.ValidateAsync(command, ct);

            //if (!validationResult.IsValid)
            //    throw new ValidationException(validationResult.Errors);

            await _repository.UpdateStatusAsync(command.GroupId, command.PermissionId, command.IsActive, ct);

            return new UpdateGroupPermissionStatusResult
            {
                GroupId = command.GroupId,
                PermissionId = command.PermissionId,
                IsActive = command.IsActive,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
