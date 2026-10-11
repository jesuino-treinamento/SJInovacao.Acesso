using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.UpdateGroupPermissionStatus;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.GroupPermissions.UpdateGroupPermissionStatus
{
    public class UpdateGroupPermissionStatusHandler : IRequestHandler<UpdateGroupPermissionStatusCommand, UpdateGroupPermissionStatusResult>
    {
        private readonly IGroupsPermissionsRepository _groupsPermissionsrepository;

        public UpdateGroupPermissionStatusHandler(IGroupsPermissionsRepository groupsPermissionsrepositoryr)
        {
            _groupsPermissionsrepository = groupsPermissionsrepositoryr;
        }

        public async Task<UpdateGroupPermissionStatusResult> Handle(UpdateGroupPermissionStatusCommand command, CancellationToken ct)
        {
            await _groupsPermissionsrepository.UpdateStatusAsync(command.GroupId, command.PermissionId, command.IsActive, ct);

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
