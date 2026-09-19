using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.UpdatePermission
{
    public class UpdatePermissionProfile : Profile
    {
        public UpdatePermissionProfile()
        {
            CreateMap<UpdatePermissionRequest, UpdatePermissionCommand>();
            CreateMap<PermissionDto, PermissionResponse>();
        }
    }
}
