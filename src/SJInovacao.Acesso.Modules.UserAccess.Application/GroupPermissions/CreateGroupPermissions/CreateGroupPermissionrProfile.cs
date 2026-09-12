using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions
{
    public class CreateGroupPermissionrProfile : Profile
    {
        public CreateGroupPermissionrProfile()
        {
            CreateMap<CreateGroupPermissionCommand, GroupPermission>()
             .ForMember(dest => dest.Permissions, opt => opt.Ignore())
             .ForMember(dest => dest.Users, opt => opt.Ignore())
             .ForMember(dest => dest.UserGroups, opt => opt.Ignore());

            CreateMap<GroupPermission, CreateGroupPermissionResult>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

           

            // Se ainda não tiver um mapeamento global de Permission para PermissionDto
            CreateMap<Permission, PermissionDto>();
        }
    }
}
