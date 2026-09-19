using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.WebAPI.Common.Request
{
    public class DocumentRequest
    {
        public string Number { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }
    }
}
