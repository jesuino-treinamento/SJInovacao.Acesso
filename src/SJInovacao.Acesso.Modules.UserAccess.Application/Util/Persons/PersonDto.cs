using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons
{
    public class PersonDto
    {
        public Guid Id { get; set; }
        public NameResult Name { get; set; } = new();
        public DocumentResult Document { get; set; } = new();
        public List<AddressDto> Addresses{ get; set; } = new();
        public List<PhoneDto> Phones { get; set; } = new();
    }

    public class DocumentResult
    {
        public string Number { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }
    }

    public class NameResult
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
    }
}
