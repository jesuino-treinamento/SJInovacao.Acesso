using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.UpdateGroupUsersPermission
{
    public class UpdateGroupUsersPermissionProfile : Profile
    {
        public UpdateGroupUsersPermissionProfile() 
        {
            // Mapeamento de User para UpdateGroupUsersPermissionResult 
            CreateMap<User, GroupUsersPermissionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupId, opt => opt.Ignore()) // Será definido manualmente
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.UserGroups.SelectMany(g => g.Group.Permissions)));

            CreateMap<UpdateGroupUsersPermissionCommand, GroupPermission>()
                  .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<User, UpdateGroupUsersPermissionCommand>();

            CreateMap<Permission, UpdateGroupUsersPermissionCommand>();

            CreateMap<GroupPermission, GroupUsersPermissionDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Permission, GroupPermissionsDto>();
        }
    }
}
