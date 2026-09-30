using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.CreateGroupAccess
{ 
    public class CreateGroupAccessProfile : Profile
    {
        public CreateGroupAccessProfile()
        {
            //CreateMap<CreateGroupUserRequest, CreateGroupUserCommand>();
            //CreateMap<CreateGroupUserResult, GroupUserResponse>();

            CreateMap<CreateGroupAccessRequest, CreateGroupPermissionCommand>();
            CreateMap<CreateGroupPermissionResult, GroupAccessResponse>();
            CreateMap<GroupPermissionsDto, GroupAccessResponse>();
        }
    }
}
