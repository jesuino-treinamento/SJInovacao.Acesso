using SJInovacao.Acesso.Modules.UserAccess.Application.Util;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users
{
    public class UserResult
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public NameResult Name { get; set; } = new();
        public DocumentResult Document { get; set; } = new();
        public List<AddressDto> Addresses { get; set; } = new();
        public List<PhoneDto> Phones { get; set; } = new();
        public StatusTypes Status { get; set; }
        public UserRole Role { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
