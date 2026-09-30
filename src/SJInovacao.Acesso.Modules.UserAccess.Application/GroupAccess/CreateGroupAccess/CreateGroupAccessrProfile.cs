using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.CreateGroupAccess
{
    public class CreateGroupAccessProfile : Profile
    {
        public CreateGroupAccessProfile()
        {
            CreateMap<CreateGroupAccessCommand, GroupPermission>()
             .ForMember(dest => dest.Permissions, opt => opt.Ignore())
             //.ForMember(dest => dest.Users, opt => opt.Ignore())
             .ForMember(dest => dest.UserGroups, opt => opt.Ignore());

            CreateMap<GroupPermission, CreateGroupAccessResult>()
                .ForMember(dest => dest.Permissions, opt => opt.MapFrom(src => src.Permissions));

           

            // Se ainda não tiver um mapeamento global de Permission para PermissionDto
            CreateMap<Permission, PermissionDto>();
        }
    }
}
