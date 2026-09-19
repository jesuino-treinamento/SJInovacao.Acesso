using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.WebAPI.Common.Response
{
    public class DocumentResponse
    {
        public string Number { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }
    }
}
