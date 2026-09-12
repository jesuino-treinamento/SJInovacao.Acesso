using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Supplier : BaseEntity//: User // Fornecedor
    {
        public Guid UserId { get; set; }
        public string StateRegistration { get; private set; } = string.Empty;
        public string MunicipalRegistration { get; private set; } = string.Empty;
        public string TradeName { get; private set; } = string.Empty;
        public DateTime? RegistrationDate { get; private set; }

        public ICollection<SupplierOrder> SupplierOrders { get;  set; } = new List<SupplierOrder>();
        public ICollection<AccountPayable> AccountsPayables { get; set; } = new List<AccountPayable>();
        public ICollection<Product> Products { get; set; } = new List<Product>();
        public User? User { get; set; }

        protected Supplier()
        {
            StateRegistration = string.Empty;
            MunicipalRegistration = string.Empty;
            TradeName = string.Empty;
        }

        public Supplier(
            string name,
            string tradeName,
            PersonType personType,
            Document document,
            string email,
            string stateRegistration,
            string municipalRegistration)
        {
            TradeName = !string.IsNullOrWhiteSpace(tradeName)
                ? tradeName
                : throw new ArgumentNullException(nameof(tradeName));

            StateRegistration = !string.IsNullOrWhiteSpace(stateRegistration)
                ? stateRegistration
                : throw new ArgumentNullException(nameof(stateRegistration));

            MunicipalRegistration = !string.IsNullOrWhiteSpace(municipalRegistration)
                ? municipalRegistration
                : throw new ArgumentNullException(nameof(municipalRegistration));

            RegistrationDate = DateTime.UtcNow;
        }

        public void UpdateStateRegistration(string registration)
        {
            if (string.IsNullOrWhiteSpace(registration))
                throw new ArgumentException("State registration cannot be empty.", nameof(registration));

            StateRegistration = registration;
        }

        public void UpdateMunicipalRegistration(string registration)
        {
            if (string.IsNullOrWhiteSpace(registration))
                throw new ArgumentException("Municipal registration cannot be empty.", nameof(registration));

            MunicipalRegistration = registration;
        }
    }
}