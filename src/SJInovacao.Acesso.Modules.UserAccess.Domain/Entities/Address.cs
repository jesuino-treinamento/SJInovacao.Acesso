using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Address : BaseEntity, IDeactivatable
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!; 
        public string Street { get; private set; } = string.Empty;      
        public string Number { get; private set; } = string.Empty;       
        public string Neighborhood { get; private set; } = string.Empty; 
        public string City { get; private set; } = string.Empty;         
        public string State { get; private set; } = string.Empty;        
        public string ZipCode { get; private set; } = string.Empty;      
        public Geolocation Geolocation { get; set; } = null!;

        public bool IsActive { get; private set; } = true;         

        // Construtor protegido para EF Core
        protected Address() { }

        public Address(string street, string number, string neighborhood, string city, string state, string zipCode, Guid personId, Geolocation geolocation)
        {
            Street = street;
            Number = number;
            Neighborhood = neighborhood;
            City = city;
            State = state;
            ZipCode = zipCode;
            PersonId = personId;
            Geolocation = geolocation;

            Validate();
        }        

        private void Validate()
        {
            if (string.IsNullOrWhiteSpace(Street))
                throw new ArgumentException("Logradouro não pode ser vazio ou nulo.", nameof(Street));

            if (string.IsNullOrWhiteSpace(Neighborhood))
                throw new ArgumentException("Bairro não pode ser vazio ou nulo.", nameof(Neighborhood));

            if (string.IsNullOrWhiteSpace(City))
                throw new ArgumentException("Cidade não pode ser vazio ou nulo.", nameof(City));

            if (string.IsNullOrWhiteSpace(State))
                throw new ArgumentException("Estado não pode ser vazio ou nulo.", nameof(State));

            if (string.IsNullOrWhiteSpace(ZipCode) || !ValidateZipCode(ZipCode))
                throw new ArgumentException("CEP inválido.", nameof(ZipCode));
        }


        private static bool ValidateZipCode(string zipCode)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{5}-\d{3}$");
            return regex.IsMatch(zipCode);
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
