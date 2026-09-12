using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.CreateGroupUsersPermission
{
    public class CreateGroupUsersPermissionProfile : Profile
    {
        public CreateGroupUsersPermissionProfile() 
        {
            // Mapeamento de User para CreateGroupUsersPermissionResult
            CreateMap<User, GroupUsersPermissionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupId, opt => opt.Ignore()) // Será definido manualmente
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.Groups.SelectMany(g => g.Permissions)));

            CreateMap<CreateGroupPermissionCommand, GroupPermission>()
                  .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<User, CreateGroupPermissionCommand>();

            CreateMap<Permission, CreateGroupPermissionCommand>();

            CreateMap<GroupPermission, GroupUsersPermissionDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Permission, GroupPermissionsDto>();
        }
    }
}
