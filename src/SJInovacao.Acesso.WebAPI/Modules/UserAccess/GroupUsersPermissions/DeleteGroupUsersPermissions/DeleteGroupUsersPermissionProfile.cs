using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupUsersPermissions.DeleteGroupUsersPermissions
{
    public class DeleteGroupUsersPermissionProfile : Profile
    {
        public DeleteGroupUsersPermissionProfile()
        {
            CreateMap<DeleteGroupUsersPermissionsRequest, DeleteGroupUsersPermissionCommand>();

            CreateMap<GroupUsersPermissionDto, DeleteGroupUsersPermissionsResponse>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.UserName))
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName)) // Novo mapeamento
            .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));
        }
    }
}
