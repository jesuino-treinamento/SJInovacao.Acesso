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

       public async Task<UpdateGroupAccessResult> Handle(UpdateGroupAccessCommand command, CancellationToken ct)
        {
            var groupAccess = await _groupAccessrepository.GetByIdAsync(command.GroupId, ct);

            if (groupAccess == null)
                throw new KeyNotFoundException($"Group with ID {command.GroupId} not found.");

            // 1️⃣ Atualizar dados básicos
            groupAccess.Name = command.Name;
            groupAccess.Description = command.Description;
            groupAccess.IsActive = command.IsActive;
            groupAccess.UpdatedAt = DateTime.UtcNow;

            // 2️⃣ Buscar permissões válidas
            var permissoesFiltradas = (await _permissionRepository.GetAllAsync(ct))
                .Where(p => command.PermissionIds.Contains(p.Id) && p.IsActive)
                .ToList();

            if (!permissoesFiltradas.Any())
                throw new InvalidOperationException("Nenhuma permissão válida encontrada!");

            // 3️⃣ Buscar vínculos existentes
            var existingLinks = (await _groupsPermissionsrepository.GetByGroupIdAsync(command.GroupId, ct)).ToList();

            // 4️⃣ Remover vínculos que não estão mais na lista
            foreach (var gp in existingLinks.Where(gp => !command.PermissionIds.Contains(gp.PermissionId)))
                await _groupsPermissionsrepository.DeleteAsync(command.GroupId, gp.PermissionId, ct);

            // 5️⃣ Adicionar vínculos novos
            foreach (var permission in permissoesFiltradas.Where(p => !existingLinks.Any(gp => gp.PermissionId == p.Id)))
            {
                var gp = new GroupsPermissions
                {
                    GroupId = command.GroupId,
                    PermissionId = permission.Id,
                    IsActive = command.IsActive,
                    CreatedAt = DateTime.UtcNow
                };

                var existingLinks1 = (await _groupsPermissionsrepository.GetAllAsync(ct))
                    .Where(g => !command.PermissionIds.Contains(gp.PermissionId))
                    .ToList();

                //groupsPermissionsrepository.DeleteAsync(command.GroupId, gp.PermissionId, ct);

                await _groupsPermissionsrepository.CreateAsync(gp, ct);
            }

            // 6️⃣ Atualizar status dos vínculos existentes
            foreach (var gp in existingLinks.Where(gp => command.PermissionIds.Contains(gp.PermissionId)))
            {
                gp.IsActive = command.IsActive;
                gp.UpdatedAt = DateTime.UtcNow;
                await _groupsPermissionsrepository.UpdateAsync(gp, ct);
            }

            // 7️⃣ Atualizar grupo
            await _groupAccessrepository.UpdateAsync(groupAccess, ct);

            return new UpdateGroupAccessResult
            {
                GroupId = command.GroupId,
                Name = command.Name,
                PermissionIds = command.PermissionIds,
                IsActive = command.IsActive
            };
        }
    }
}
