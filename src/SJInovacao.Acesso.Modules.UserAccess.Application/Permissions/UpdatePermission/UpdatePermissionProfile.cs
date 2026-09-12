using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission
{
    public class UpdatePermissionProfile : Profile
    {
        public UpdatePermissionProfile()
        {
            CreateMap<UpdatePermissionCommand, Permission>();
            CreateMap<Permission, PermissionDto>();

        }
    }
}
