using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.ListPermissions
{
    public class ListPermissionProfile : Profile
    {
        public ListPermissionProfile()
        {
            // Mapeia o DTO de Application (PermissionResult) para o Response da API
            CreateMap<PermissionDto, PermissionResponse>();
        }
    }
}
