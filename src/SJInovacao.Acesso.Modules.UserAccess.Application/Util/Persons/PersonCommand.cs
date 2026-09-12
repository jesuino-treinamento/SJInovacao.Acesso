using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Addresses;
using SJInovacao.Acesso.Modules.UserAccess.Application.Util.Phones;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Util.Persons
{
    public class PersonCommand : IRequest<PersonDto>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DocumentNumber { get; set; } = string.Empty;
        public PersonType PersonType { get; set; }
        public List<AddressCommand> Addresses { get; set; } = new();
        public List<PhoneCommand> Phones { get; set; } = new();
    }
}