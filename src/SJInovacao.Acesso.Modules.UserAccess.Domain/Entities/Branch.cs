using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Branch : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Location { get; private set; } = string.Empty;

        public ICollection<Sale> Sales { get; private set; }
        public ICollection<Employee> Employees { get; private set; }
        public ICollection<Product> Products { get; private set; }
        public ICollection<Customer> Customers { get; private set; }
        public ICollection<CashRegister> CashRegisters { get; private set; }
        public ICollection<CustomerOrder> CustomerOrders { get; private set; } 

        protected Branch()
        {
            Sales = new List<Sale>();
            Employees = new List<Employee>();
            Products = new List<Product>();
            Customers = new List<Customer>();
            CashRegisters = new List<CashRegister>();
            CustomerOrders = new List<CustomerOrder>();
        }

        public Branch(string name, string location)
        {
            Name = name;
            Location = location;

            Sales = new List<Sale>();
            Employees = new List<Employee>();
            Products = new List<Product>();
            Customers = new List<Customer>();
            CashRegisters = new List<CashRegister>();
            CustomerOrders = new List<CustomerOrder>();
        }
    }
}
