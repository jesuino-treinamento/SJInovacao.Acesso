using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using System.Text.RegularExpressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.UpdatePermission
{
    public class UpdatePermissionProfile : Profile
    {
        public UpdatePermissionProfile()
        {
            // Permission ↔ PermissionDto
            CreateMap<Permission, PermissionDto>().ReverseMap();

            // Group ↔ GroupDto
           // CreateMap<Group, Group>().ReverseMap();

            // User ↔ UserDto
            CreateMap<User, UserPermissionsDto>().ReverseMap();

            // UsersGroupsPermissions ↔ GroupUsersPermissionDto
            CreateMap<UsersGroupsPermissions, GroupUsersPermissionDto>()
                .ForMember(dest => dest.GroupId, opt => opt.MapFrom(src => src.GroupId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PermissionIds, opt => opt.MapFrom(src => src.PermissionId))
                .ReverseMap();

            // Command ↔ Entidade (se necessário)
            CreateMap<UpdatePermissionCommand, Permission>();

        }
    }
}
