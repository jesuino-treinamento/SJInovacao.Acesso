using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Customer : BaseEntity
    {
        //public Guid Id { get; set; }  // PK da própria entidade Customer
        public Guid UserId { get; set; }
        public User? User { get; set; } = new();

        public string DocumentNumber => User?.Document?.Number ?? string.Empty;

        // public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<Product> Products { get; set; } = new List<Product>();

        public ICollection<Billing> Billings { get; set; } = new List<Billing>();

        public ICollection<CustomerOrder> CustomerOrders { get; set; } = new List<CustomerOrder>();

        [NotMapped]
        public Name? Name => User?.Name;

        public Guid BranchId { get; set; }
        public Branch? Branch { get; set; }
        //[NotMapped]



        [NotMapped]
        public Address? Address => User?.Addresses?.FirstOrDefault();

        [NotMapped]
        public Phone? Phones => User?.Phones?.FirstOrDefault();

        protected Customer() { }

        public Customer(Guid branchId, Guid userId)
        {
            BranchId = branchId;
            UserId = userId;
        }
    }
}
