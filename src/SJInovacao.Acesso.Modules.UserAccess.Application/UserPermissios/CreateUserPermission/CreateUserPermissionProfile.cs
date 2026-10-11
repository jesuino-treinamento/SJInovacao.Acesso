using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissios.CreateUserPermission;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.CreateUserPermission
{
    public class CreateUserPermissionProfile : Profile
    {
        public CreateUserPermissionProfile()
        {
            CreateMap<CreateUserPermissionCommand, UserPermission>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Permission, opt => opt.Ignore());
            CreateMap<UserPermission, CreateUserPermissionResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId));
            CreateMap<Permission, PermissionDto>();
        }
    }
}
