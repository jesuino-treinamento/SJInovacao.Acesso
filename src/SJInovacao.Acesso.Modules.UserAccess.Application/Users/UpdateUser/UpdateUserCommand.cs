using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.UpdateUser
{
    public class UpdateUserCommand : IRequest<UserResult>
    {
        public Guid Id { get; internal set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public Name Name { get; set; } = new();
        public Document Document { get; set; } = new();
        public PersonType PersonType { get; set; }
        public List<AddressCommand> Addresses { get; set; } = new();
        public List<PhoneCommand> Phones { get; set; } = new();
        public string Email { get; set; } = string.Empty;
        public StatusTypes Status { get; set; }
        public UserRole Role { get; set; }

    }
}
