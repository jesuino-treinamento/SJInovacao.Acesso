using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.CreateGroupUsersPermissions
{
    public class CreateGroupUsersPermissionProfile : Profile
    {
        public CreateGroupUsersPermissionProfile()
        {
            CreateMap<CreateGroupUsersPermissionsRequest, CreateGroupUsersPermissionCommand>();

            CreateMap<GroupUsersPermissionDto, CreateGroupUsersPermissionsResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName)) // Novo mapeamento
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}
