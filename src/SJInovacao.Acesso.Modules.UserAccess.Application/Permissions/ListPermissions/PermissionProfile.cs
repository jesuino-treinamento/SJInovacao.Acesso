using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.ListPermissions
{
    public class PermissionProfile : Profile
    {
        public PermissionProfile()
        {
            CreateMap<Permission, PermissionDto>();
            CreateMap<CreatePermissionCommand, Permission>();
           // CreateMap<Permission, CreatePermissionResult>();
        }
    }
}
