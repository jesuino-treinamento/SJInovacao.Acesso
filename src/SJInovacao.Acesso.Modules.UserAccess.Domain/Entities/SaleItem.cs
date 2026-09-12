using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Exceptions;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class SaleItem : BaseEntity
    {
        public Guid ProductId { get; set; }
        public Sale? Sale { get; set; }
        public Guid SaleId { get; set; }
        public Product? Product { get; set; } = null!;
        public string ProductName { get; set; } = string.Empty;
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal Discount { get; set; }

        public DateTime? CancellationDate { get; set; } = null;

        public bool IsCancelled { get; set; }
        public SaleItem() { }

        public SaleItem(Guid productId, string? productName, decimal unitPrice, int quantity, decimal discount)
        {
            ProductId = productId;
            ProductName = productName ?? string.Empty;
            UnitPrice = unitPrice;
            Quantity = quantity;
            Discount = discount;
            TotalPrice = CalculateTotalAmount();
        }
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

        //public void ApplyDiscount()
        //{
        //    if (DiscountCalculator.ValidateItemQuantity(Quantity))
        //    {
        //        Discount = DiscountCalculator.CalculateDiscount(Quantity, UnitPrice);
        //        TotalPrice = CalculateTotalAmount();
        //    }
        //    else
        //    {
        //        throw new InvalidOperationException("The quantity of items cannot be greater than 20.");
        //    }
        //}
    }
}
