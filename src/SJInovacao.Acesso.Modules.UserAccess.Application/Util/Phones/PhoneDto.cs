using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones
{
    public class PhoneDto
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public PhoneType Type { get; set; }
    }
}
