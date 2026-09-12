using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Represents a customer order in the system
    /// </summary>
    public class CustomerOrder : BaseEntity, IDeactivatable
    {
        #region Properties

        /// <summary>
        /// Order date
        /// </summary>
        public DateTime Date { get; private set; }

        /// <summary>
        /// Customer ID (foreign key)
        /// </summary>
        public Guid CustomerId { get; private set; }

        /// <summary>
        /// Navigation property to Customer
        /// </summary>
        public Customer Customer { get; private set; } = null!;

        /// <summary>
        /// Branch ID (optional - foreign key)
        /// </summary>
        public Guid? BranchId { get; private set; }

        /// <summary>
        /// Navigation property to Branch
        /// </summary>
        public Branch? Branch { get; private set; }

        /// <summary>
        /// Company ID (optional - foreign key)
        /// </summary>
        //public int? CompanyId { get; private set; }

        ///// <summary>
        ///// Navigation property to Company
        ///// </summary>
        //public Company? Company { get; private set; }

        /// <summary>
        /// Collection of order items
        /// </summary>
        public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

        /// <summary>
        /// Indicates if the order is active
        /// </summary>
        public bool IsActive { get; private set; }

        #endregion

        #region Constructors

        // Construtor privado para ORM (Entity Framework)
        protected CustomerOrder() { }

        /// <summary>
        /// Creates a new active order
        /// </summary>
        /// <param name="date">Order date</param>
        public CustomerOrder(DateTime date)
        {
            IsActive = true;
            Date = date;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds an item to the order
        /// </summary>
        /// <param name="item">Item to be added</param>
        /// <exception cref="ArgumentException">Thrown when quantity is invalid</exception>
        public void AddItem(OrderItem item)
        {
            // Validação básica da quantidade
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            OrderItems.Add(item);
        }

        /// <summary>
        /// Removes an item from the order
        /// </summary>
        /// <param name="item">Item to be removed</param>
        /// <exception cref="ArgumentException">Thrown when quantity is invalid</exception>
        public void RemoveItem(OrderItem item)
        {
            // Validação básica da quantidade
            if (item.Quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero.");

            OrderItems.Remove(item);
        }

        /// <summary>
        /// Deactivates the order (marks as inactive)
        /// </summary>
        public void Deactivate()
        {
            // Método idempotente - não faz nada se já estiver inativo
            if (!IsActive) return;

            IsActive = false;
        }

        /// <summary>
        /// Activates the order (marks as active)
        /// </summary>
        public void Activate()
        {
            // Método idempotente - não faz nada se já estiver ativo
            if (IsActive) return;

            IsActive = true;
        }

        #endregion
    }
}