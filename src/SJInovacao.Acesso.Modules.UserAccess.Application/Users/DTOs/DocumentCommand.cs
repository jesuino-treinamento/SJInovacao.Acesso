using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Users.DTOs
{
    public class DocumentCommand
    {
        public string Number { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }
    }
}
