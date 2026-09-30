using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.DeleteGroupAccess;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.DeleteGroupAccess
{
    public class DeleteGroupAccessProfile : Profile
    {
        public DeleteGroupAccessProfile()
        {
            CreateMap<DeleteGroupAccessRequest, DeleteGroupAccessCommand>();

            CreateMap<GroupUsersPermissionDto, DeleteGroupAccessResponse>()
            .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
            .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.GroupName));
        }
    }
}
