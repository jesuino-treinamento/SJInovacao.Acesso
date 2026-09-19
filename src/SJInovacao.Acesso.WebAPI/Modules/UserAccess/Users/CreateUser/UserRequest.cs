using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.WebAPI.Common.Request;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser
{
    public class UserRequest
    {
        public Guid Id { get; internal set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public NameRequest Name { get; set; } = new();
        public DocumentRequest Document { get; set; } = new();
        public List<AddressRequest> Addresses { get; set; } = new();
        public List<PhoneRequest> Phones { get; set; } = new();
        public StatusTypes Status { get; set; }
        public UserRole Role { get; set; }

        public bool UseFakeData { get; set; } = false;
    }
}
