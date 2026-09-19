using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.CreateGroupPermissions;
using SJInovacao.Acesso.WebAPI.Features.GroupPermissions.CreateGroupPermission;
using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.CreateGroupPermission;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupPermissions.GroupPermissions
{ 
    public class CreateGroupPermissionProfile : Profile
    {
        public CreateGroupPermissionProfile()
        {
            //CreateMap<CreateGroupUserRequest, CreateGroupUserCommand>();
            //CreateMap<CreateGroupUserResult, GroupUserResponse>();

            CreateMap<CreateGroupPermissionRequest, CreateGroupPermissionCommand>();
            CreateMap<CreateGroupPermissionResult, GroupPermissionResponse>();
            CreateMap<GroupPermissionsDto, GroupPermissionResponse>();
        }
    }
}
