using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Represents an item within a customer order
    /// </summary>
    public class OrderItem : BaseEntity
    {
        #region Properties

        /// <summary>
        /// Order ID (foreign key)
        /// </summary>
       // public int OrderId { get; private set; }

        private bool _isCancelled;

        public Guid CustomerOrderId { get; set; }
        /// <summary>
        /// Navigation property to Order
        /// </summary>
        public CustomerOrder CustomerOrder { get; private set; } = null!;

        /// <summary>
        /// Product ID (foreign key)
        /// </summary>
        public Guid ProductId { get; private set; }

        /// <summary>
        /// Navigation property to Product
        /// </summary>
        public Product Product { get; private set; } = null!;

        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// Item quantity
        /// </summary>
        public int Quantity { get; private set; }

        /// <summary>
        /// Unit price at the time of order
        /// </summary>
        public decimal UnitPrice { get; private set; }

        public decimal TotalPrice { get; set; }

        public decimal Discount { get; set; }

        public DateTime? CancellationDate { get; set; } = null;

        public bool IsCancelled { get; set; }
       

        #endregion

        #region Constructors

        // Construtor privado para ORM (Entity Framework)
        protected OrderItem() { }

        /// <summary>
        /// Creates a new active order item
        /// </summary>
        /// <param name="productId">Product ID</param>
        /// <param name="quantity">Item quantity</param>
        /// <param name="unitPrice">Unit price</param>
        public OrderItem(Guid productId, string productName, decimal unitPrice, int quantity, decimal discount)
        {
            ProductId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Discount = discount;
            TotalPrice = CalculateTotalAmount();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Calculates the total value for this order item
        /// </summary>
        /// <returns>Total value (quantity × unit price)</returns>
        public void Cancel()
        {
            if (IsCancelled)
            {
                throw new DomainException("Item is already canceled");
            }

            IsCancelled = true;
            CancellationDate = DateTime.UtcNow;
        }

        public decimal CalculateTotalAmount()
        {
            var grossTotal = UnitPrice * Quantity;
            var netTotal = grossTotal - Discount;

            if (netTotal < 0)
                netTotal = 0;

            TotalPrice = Math.Round(netTotal, 2, MidpointRounding.AwayFromZero);
            return TotalPrice;
        }       

        #endregion
    }
}