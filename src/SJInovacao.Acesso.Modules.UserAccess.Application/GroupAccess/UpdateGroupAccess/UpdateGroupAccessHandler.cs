using AutoMapper;
using FluentValidation;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.UpdateGroupAccess;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Repositories;
using System.Threading;

namespace SJInovacao.Acesso.Modules.UserAccess.GroupAccess.UpdateGroupAccess
{ 
    public class UpdateGroupAccessHandler : IRequestHandler<UpdateGroupAccessCommand, UpdateGroupAccessResult>
    {
        private readonly IGroupPermissionRepository _groupAccessrepository;
        private readonly IPermissionRepository _permissionRepository;
        private readonly IGroupsPermissionsRepository _groupsPermissionsrepository;
        private readonly IMapper _mapper;

        public UpdateGroupAccessHandler(IGroupPermissionRepository groupAccessrepository,
            IPermissionRepository permissionRepository,
            IGroupsPermissionsRepository groupsPermissionsrepository, IMapper mapper)
        {
            _groupAccessrepository = groupAccessrepository;
            _permissionRepository = permissionRepository;
            _groupsPermissionsrepository = groupsPermissionsrepository;
            _mapper = mapper;
        }

        //public async Task<UpdateGroupAccessResult> Handle(UpdateGroupAccessCommand command, CancellationToken ct)
        // {
        //     var groupAccess = await _groupAccessrepository.GetByIdAsync(command.GroupId, ct);

        //     if (groupAccess == null)
        //         throw new KeyNotFoundException($"Group with ID {command.GroupId} not found.");

        //     groupAccess.Name = command.Name;
        //     groupAccess.Description = command.Description;
        //     groupAccess.IsActive = command.IsActive;
        //     groupAccess.UpdatedAt = DateTime.UtcNow;

        //     var permissoesFiltradas = (await _permissionRepository.GetAllAsync(ct))
        //         .Where(p => command.PermissionIds.Contains(p.Id) && p.IsActive)
        //         .ToList();

        //     if (!permissoesFiltradas.Any())
        //         throw new InvalidOperationException("Nenhuma permissão válida encontrada!");

        //     var existingLinks = (await _groupsPermissionsrepository.GetByGroupIdAsync(command.GroupId, ct)).ToList();

        //     foreach (var gp in existingLinks.Where(gp => command.PermissionIds.Contains(gp.PermissionId)))
        //     {
        //         groupAccess.Permissions = permissoesFiltradas;// gp.Permission.GroupsPermissions;
        //         groupAccess.GroupsPermissions.Add(groupAccess.Permissions);
        //     }
        //     await _groupAccessrepository.UpdateAsync(groupAccess, ct);

        //     return new UpdateGroupAccessResult
        //     {
        //         GroupId = command.GroupId,
        //         Name = command.Name,
        //         Description = command.Description,
        //         PermissionIds = command.PermissionIds,
        //         IsActive = command.IsActive
        //     };
        // }

        public async Task<UpdateGroupAccessResult> Handle(UpdateGroupAccessCommand command, CancellationToken ct)
        {
            var groupAccess = await _groupAccessrepository.GetByIdAsync(command.GroupId, ct);

            if (groupAccess == null)
                throw new KeyNotFoundException($"Group with ID {command.GroupId} not found.");

            // Atualiza dados básicos
            groupAccess.Name = command.Name;
            groupAccess.Description = command.Description;
            groupAccess.IsActive = command.IsActive;
            groupAccess.UpdatedAt = DateTime.UtcNow;

            // Filtra permissões válidas
            var permissoesFiltradas = (await _permissionRepository.GetAllAsync(ct))
                .Where(p => command.PermissionIds.Contains(p.Id) && p.IsActive)
                .ToList();

            if (!permissoesFiltradas.Any())
                throw new InvalidOperationException("Nenhuma permissão válida encontrada!");

            var existingLinks = (await _groupsPermissionsrepository.GetByGroupIdAsync(command.GroupId, ct)).ToList();

            foreach (var gp in existingLinks.Where(gp => !command.PermissionIds.Contains(gp.PermissionId)))
            {
                groupAccess.GroupsPermissions.Remove(gp);
            }
            await _groupAccessrepository.UpdateAsync(groupAccess, ct);

            return new UpdateGroupAccessResult
            {
                GroupId = command.GroupId,
                Name = command.Name,
                Description = command.Description,
                PermissionIds = command.PermissionIds,
                IsActive = command.IsActive
            };
        }

    }
}
