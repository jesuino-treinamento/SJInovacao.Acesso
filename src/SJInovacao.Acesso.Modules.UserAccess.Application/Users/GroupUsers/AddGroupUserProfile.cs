using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.GroupUsers
{
    public class AddGroupUserProfile : Profile
    {
        public AddGroupUserProfile()
        {
            CreateMap<AddGroupUserCommand, Permission>();
        }
    }
}
