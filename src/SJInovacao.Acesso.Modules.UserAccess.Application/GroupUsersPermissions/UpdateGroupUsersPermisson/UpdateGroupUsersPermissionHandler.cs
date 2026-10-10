using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission
{
    public class UpdateGroupUsersPermissionHandler
        : IRequestHandler<UpdateGroupUsersPermissionCommand, GroupUsersPermissionDto>
    {
        private const string UnknownGroupName = "Desconhecido";

        private readonly IUserRepository _userRepository;
        private readonly IUsersGroupsPermissionsRepository _ugpRepository;
        private readonly IMapper _mapper;

        public UpdateGroupUsersPermissionHandler(
            IUserRepository userRepository,
            IUsersGroupsPermissionsRepository ugpRepository,
            IMapper mapper)
        {
            _userRepository = userRepository;
            _ugpRepository = ugpRepository;
            _mapper = mapper;
        }

        public async Task<GroupUsersPermissionDto> Handle(
            UpdateGroupUsersPermissionCommand request,
            CancellationToken cancellationToken)
        {
            var existingUser = await _userRepository.GetByIdAsync(request.UserId, cancellationToken)
                ?? throw new DomainException($"Usuário com ID {request.UserId} não encontrado.");

            var existingLinks = await _ugpRepository.GetByUserGroupAsync(
                request.UserId, request.GroupAccessId, cancellationToken);

            if (existingLinks.Count == 0)
                throw new DomainException(
                    $"Nenhum vínculo encontrado para o usuário {request.UserId} no grupo {request.GroupAccessId}.");

            var permissionsUpdated = await _ugpRepository.UpdateUserGroupPermissionsAsync(
                request.UserId,
                request.GroupAccessId,
                request.UserIsActive,
                request.PermissionIds,
                request.PermissionIsActive,
                cancellationToken);

            var firstLink = existingLinks[0];

            return new GroupUsersPermissionDto
            {
                UserId = request.UserId,
                UserName = existingUser.Name?.ToString() ?? string.Empty,
                GroupId = request.GroupAccessId,
                GroupName = firstLink.Group?.Name ?? UnknownGroupName,
                Permissions = _mapper.Map<List<PermissionDto>>(
                    permissionsUpdated ?? Array.Empty<Permission>()),
                UserIsActive = request.UserIsActive,
                PermissionIsActive = request.PermissionIsActive ?? true
            };
        }
    }
}
