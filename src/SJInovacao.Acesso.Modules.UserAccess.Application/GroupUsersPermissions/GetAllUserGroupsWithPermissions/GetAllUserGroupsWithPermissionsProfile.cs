using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUserGroupsWithPermissionsProfile : Profile
    {
        public GetAllUserGroupsWithPermissionsProfile()
        {
            CreateMap<Permission, PermissionDto>().ReverseMap();
            CreateMap<User, UserPermissionsDto>().ReverseMap();
            CreateMap<UserGroup, UserGroupDTO>().ReverseMap();
            CreateMap<UsersGroupsPermissions, GroupUsersPermissionResult>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserIsActive, opt => opt.MapFrom(src => src.User.Status))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.Name.ToString()))
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Group.Name))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => new PermissionDto
                {
                    Id = src.Permission.Id,
                    Name = src.Permission.Name,
                    Description = src.Permission.Description,
                    IsActive = src.Permission.IsActive                    
                }))
                .ReverseMap();
        }
    }
}
