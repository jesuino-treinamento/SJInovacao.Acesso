using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.UpdateUserPermissionStatus
{
    public class UpdateUserPermissionStatusHandler : IRequestHandler<UpdateUserPermissionStatusCommand, UpdateUserPermissionStatusResult>
    {
        private readonly IUserPermissionRepository _repository;
        private readonly IMapper _mapper;

        public UpdateUserPermissionStatusHandler(IUserPermissionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<UpdateUserPermissionStatusResult> Handle(UpdateUserPermissionStatusCommand command, CancellationToken ct)
        {
            //var validator = new UpdateUserPermissionStatusValidator();
            //var validationResult = await validator.ValidateAsync(command, ct);

            //if (!validationResult.IsValid)
            //    throw new ValidationException(validationResult.Errors);

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
