using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission
{
    public class CreatePermissionProfile : Profile
    {
        public CreatePermissionProfile()
        {
            CreateMap<CreatePermissionRequest, CreatePermissionCommand>();
            CreateMap<PermissionDto, PermissionResponse>();
        }
    }
}
