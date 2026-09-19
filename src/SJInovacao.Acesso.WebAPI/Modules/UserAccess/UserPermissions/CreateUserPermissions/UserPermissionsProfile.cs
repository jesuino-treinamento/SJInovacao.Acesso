using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.Permissions.CreatePermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.UserPermissions.CreateUserPermissions
{
    public class UserPermissionsProfile : Profile
    {
        public UserPermissionsProfile()
        {
            // Mapeamentos de entrada (requests) -> (normalmente) comandos/queries.
            // Não referencio comandos aqui para evitar acoplamento direto — controllers podem mapear requests manualmente.
            // Exemplo (descomentado) se os comandos existirem:
            // CreateMap<CreateUserPermissionRequest, CreateUserPermissionCommand>();
            // CreateMap<RemoveUserPermissionRequest, RemoveUserPermissionCommand>();
            // CreateMap<GetUserPermissionsRequest, GetUserPermissionsQuery>();
            // CreateMap<UpdateUserPermissionStatusRequest, UpdateUserPermissionStatusCommand>();

            // Mapeamentos de saída (Application DTOs -> Responses)
            CreateMap<PermissionDto, UserPermissionResponse>();
            CreateMap<GroupUsersPermissionDto, UserPermissionResponse>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}
