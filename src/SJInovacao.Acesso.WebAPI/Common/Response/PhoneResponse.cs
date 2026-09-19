using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.WebAPI.Common.Response
{
    public class PhoneResponse
    {
        public Guid Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public PersonType Type { get; set; } // "Mobile", "Home", "Work"
    }
}