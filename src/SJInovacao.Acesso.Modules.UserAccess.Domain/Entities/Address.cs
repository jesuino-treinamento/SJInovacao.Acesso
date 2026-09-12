using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um Endereço no sistema
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class Address : BaseEntity, IDeactivatable
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; }
        public string Street { get; private set; } = null!;       // Logradouro do endereço
        public string Number { get; private set; } = null!;       // Número do endereço
        public string Neighborhood { get; private set; } = null!; // Bairro
        public string City { get; private set; } = null!;         // Cidade
        public string State { get; private set; } = null!;        // Estado (UF)
        public string ZipCode { get; private set; } = null!;      // CEP no formato 00000-000       
        public Geolocation Geolocation { get; set; }

        public bool IsActive { get; private set; } = true;         // Indica se o endereço está ativo

        // Construtor privado para EF Core
        public Address() { }

        /// <summary>
        /// Construtor para criar um novo endereço
        /// </summary>
        /// <param name="street">Logradouro (não pode ser vazio)</param>
        /// <param name="number">Número</param>
        /// <param name="neighborhood">Bairro (não pode ser vazio)</param>
        /// <param name="city">Cidade (não pode ser vazia)</param>
        /// <param name="state">Estado (não pode ser vazio)</param>
        /// <param name="zipCode">CEP (deve estar no formato correto)</param>
        /// <param name="personId">ID da pessoa associada</param>
        /// <exception cref="ArgumentException">Lançada quando algum parâmetro é inválido</exception>
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


        /// <summary>
        /// Valida se o CEP está no formato correto (00000-000)
        /// </summary>
        /// <param name="zipCode">CEP a ser validado</param>
        /// <returns>True se válido, False caso contrário</returns>
        private static bool ValidateZipCode(string zipCode)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^\d{5}-\d{3}$");
            return regex.IsMatch(zipCode);
        }

        /// <summary>
        /// Desativa o endereço (marca como inativo)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o endereço (marca como ativo)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
