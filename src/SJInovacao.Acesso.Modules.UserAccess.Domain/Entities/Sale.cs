using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Sale : BaseEntity
    {
        private bool _isCancelled;
        public Guid CustomerId { get; set; }
        public Customer? Customer { get; set; }

        public Guid BranchId { get; set; }
        public Branch? Branch { get; set; }

        public string? SaleNumber { get; set; } = string.Empty;


        public DateTime SaleDate { get; set; }


        public decimal TotalAmount { get; set; }

        public decimal TotalDiscount { get; set; }
        public DateTime? ModifiedDate { get; set; }

        public virtual ICollection<SaleItem> Items { get; set; } = new List<SaleItem>();

        public SaleStatus Status { get; set; }
        public DateTime? CancellationDate { get; set; }
        public bool IsCancelled
        {
            get => _isCancelled;
            private set
            {
                _isCancelled = value;
                Status = value ? SaleStatus.Cancelled : SaleStatus.Completed;
            }
        }

        //public decimal TotalAmount { get; set; }

        public Sale() { }

        public Sale(Guid customerId, Guid branchId, List<SaleItem> items)
        {
            if (customerId == Guid.Empty)
                throw new ArgumentException("CustomerId cannot be empty", nameof(customerId));

            if (branchId == Guid.Empty)
                throw new ArgumentException("BranchId cannot be empty", nameof(branchId));

            if (items == null || !items.Any())
                throw new ArgumentException("Sale must have at least one item", nameof(items));

            CustomerId = customerId;
            BranchId = branchId;
            SaleNumber = GenerateSaleNumber();
            SaleDate = DateTime.UtcNow;
            Status = SaleStatus.Pending;

            foreach (var item in items)
            {
                AddItem(item);
            }

            CalculateTotals();
            IsCancelled = false;
        }

        public string GenerateSaleNumber()
        {
            return $"SALE-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public void AddItem(SaleItem item)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot add items to a cancelled sale.");

            Items.Add(item);
            TotalAmount = CalculateTotalAmount();
        }

        public void RemoveItem(SaleItem item)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot remove items from a cancelled sale.");

            Items.Remove(item);
            TotalAmount = CalculateTotalAmount();
        }

        public void UpdateTotalAmount(decimal totalAmount)
        {
            TotalAmount = totalAmount;
            ModifiedDate = DateTime.UtcNow;
        }

        public void UpdateTotalDiscount(decimal totalDiscount)
        {
            TotalDiscount = totalDiscount;
            ModifiedDate = DateTime.UtcNow;
        }

        public void CancelSale()
        {
            IsCancelled = true;
            CancellationDate = DateTime.UtcNow;
            ModifiedDate = DateTime.UtcNow;

            foreach (var item in Items)
            {
                if (!item.IsCancelled)
                    item.Cancel();
            }
        }

        public decimal CalculateTotalAmount()
        {
            return Items.Sum(item => item.CalculateTotalAmount());
        }

        public void CompleteSale()
        {
            if (Status == SaleStatus.Completed)
                return;

            if (Status == SaleStatus.Cancelled)
                throw new InvalidOperationException("Cannot complete a cancelled sale.");

            Status = SaleStatus.Completed;
            ModifiedDate = DateTime.UtcNow;
        }

        public void CalculateTotals()
        {
            TotalAmount = Items.Sum(item => item.TotalPrice);
            TotalDiscount = Items.Sum(item => item.Discount);
            ModifiedDate = DateTime.UtcNow;
        }

        public void UpdateSale(DateTime saleDate, Guid customerId, Guid branchId, List<SaleItem> updatedItems)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot update a cancelled sale.");

            if (updatedItems == null || !updatedItems.Any())
                throw new ArgumentException("Sale must have at least one item.", nameof(updatedItems));

            SaleDate = saleDate;
            CustomerId = customerId;
            BranchId = branchId;

            Items.Clear();

            foreach (var item in updatedItems)
            {
                Items.Add(item);
            }

            CalculateTotals();

            ModifiedDate = DateTime.UtcNow;
        }

        public void ModifyDate(DateTime saleDate)
        {
            ModifiedDate = DateTime.UtcNow;
        }
    }
}
