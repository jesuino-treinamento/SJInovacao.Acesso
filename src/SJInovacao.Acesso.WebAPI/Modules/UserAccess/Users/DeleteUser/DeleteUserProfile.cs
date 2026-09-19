using AutoMapper;
using SJInovacao.Acesso.Modules.UserAccess.Application.Users.DeleteUser;
using static System.Net.Mime.MediaTypeNames;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.DeleteUser
{
    public class DeleteUserProfile : Profile
    {
        public DeleteUserProfile()
        {
            CreateMap<Guid,  DeleteUserCommand>()
                .ConstructUsing(id => new DeleteUserCommand(id));
        }
    }
}
