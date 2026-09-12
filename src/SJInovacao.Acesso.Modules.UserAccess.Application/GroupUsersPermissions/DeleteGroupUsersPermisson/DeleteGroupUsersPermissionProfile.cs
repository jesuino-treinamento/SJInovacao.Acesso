using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.DeleteGroupUsersPermission
{
    public class DeleteGroupUsersPermissionProfile : Profile
    {
        public DeleteGroupUsersPermissionProfile() 
        {
            // Mapeamento de User para DeleteGroupUsersPermissionResult 
            CreateMap<User, GroupUsersPermissionDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.GroupId, opt => opt.Ignore()) // Será definido manualmente
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name.ToString()))
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src =>
                    src.Groups.SelectMany(g => g.Permissions)));

            CreateMap<DeleteGroupUsersPermissionCommand, GroupPermission>()
                  .ForMember(dest => dest.Permissions, opt => opt.Ignore());

            CreateMap<User, DeleteGroupUsersPermissionCommand>();

            CreateMap<Permission, DeleteGroupUsersPermissionCommand>();

            CreateMap<GroupPermission, GroupUsersPermissionDto>()
                .ForMember(dest => dest.GroupName, opt => opt.MapFrom(src => src.Name));

            CreateMap<Permission, GroupPermissionsDto>();
        }
    }
}
