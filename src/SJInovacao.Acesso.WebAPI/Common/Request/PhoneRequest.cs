using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.WebAPI.Common.Request
{
    public class PhoneRequest
    {
        public Guid Id { get; internal set; }
        public string Number { get; set; } = string.Empty;
        public PersonType Type { get; set; } // "Mobile", "Home", "Work"
    }
}