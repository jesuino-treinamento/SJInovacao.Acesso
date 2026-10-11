using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Phone : BaseEntity, IDeactivatable
    {
        public string Number { get; private set; } = string.Empty;      
        public PhoneType Type { get; private set; }               
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!; 
        public bool IsActive { get; private set; } = true; 
        public Phone() { }
        public Phone(string number, PhoneType type, Person person)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Número de telefone inválido.");

            if (person == null)
                throw new ArgumentNullException(nameof(person), "Usuário associado é obrigatório.");

            Number = number;
            Type = type;
            Person = person;
            PersonId = person.Id;
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}