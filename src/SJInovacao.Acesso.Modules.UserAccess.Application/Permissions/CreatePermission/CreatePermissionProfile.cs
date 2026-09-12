using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission
{
    public class CreatePermissionProfile : Profile
    {
        public CreatePermissionProfile()
        {
            CreateMap<CreatePermissionCommand, Permission>();
            CreateMap<Permission, PermissionDto>();

        }
    }
}
