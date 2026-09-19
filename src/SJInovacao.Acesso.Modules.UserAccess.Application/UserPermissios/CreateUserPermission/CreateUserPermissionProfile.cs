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
            // COMMAND → DOMAIN
            CreateMap<CreateUserPermissionCommand, UserPermission>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Permission, opt => opt.Ignore());

            // DOMAIN → RESULT
            CreateMap<UserPermission, CreateUserPermissionResult>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.PermissionId, opt => opt.MapFrom(src => src.PermissionId));
                //.ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                //.ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                //.ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt));

            // DOMAIN → DTO (caso queira expor detalhes da permissão)
            CreateMap<Permission, PermissionDto>();
        }
    }
}
