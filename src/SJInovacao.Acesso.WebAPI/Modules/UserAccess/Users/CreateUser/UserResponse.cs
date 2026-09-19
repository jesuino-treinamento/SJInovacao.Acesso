using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.WebAPI.Common.Response;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.Users.CreateUser
{
    public class UserResponse
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public NameResponse Name { get; set; } = new();
        public DocumentResponse Document { get; set; } = new();
        public List<AddressResponse> Addresses { get; set; } = new();
        public List<PhoneResponse> Phones { get; set; } = new();
        public StatusTypes Status { get; set; }
        public UserRole Role { get; set; }
    }
}
