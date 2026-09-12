using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Person : BaseEntity
    {
        public Document Document { get; set; }           
        public Name Name { get; protected set; } = null!;          
        public ICollection<Phone> Phones { get; set; } = null!;      
        public ICollection<Address> Addresses { get; set; } = null!;
        protected Person() 
        { 
            Name = new Name();
            Document = new Document();
            Phones = new List<Phone>();
            Addresses = new List<Address>();
        }
        public Person(Name name, Document document)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Document = document ?? throw new ArgumentNullException(nameof(document));
            Phones = new List<Phone>();
            Addresses = new List<Address>();
        }
        /// <summary>
        /// Adiciona um telefone à pessoa
        /// </summary>
        /// <param name="phone">Telefone a ser adicionado</param>
        public void AddPhone(Phone phone)
        {
            Phones.Add(phone ?? throw new ArgumentNullException(nameof(phone)));
        }

        public void ClearPhones()
        {
            Phones.Clear();
        }

        public void ClearAddresses()
        {
            Addresses.Clear();
        }

        /// <summary>
        /// Adiciona um endereço à pessoa
        /// </summary>
        /// <param name="address">Endereço a ser adicionado</param>
        public void AddAddress(Address address)
        {
            Addresses.Add(address ?? throw new ArgumentNullException(nameof(address)));
        }
        public void UpdateName(string FirstName, string LastName)
        {
            Name = new Name(
                FirstName ?? throw new ArgumentNullException(nameof(FirstName)),
                LastName ?? throw new ArgumentNullException(nameof(LastName)));
        }

        public void UpdateDocument(string number, PersonType personType)
        {
            Document = new Document(number, personType);
        }
    }
}

