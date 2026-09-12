//using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

//namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
//{
//    /// <summary>
//    /// Representa um caixa ou ponto de venda no sistema
//    /// </summary>
//    public class CashRegister : BaseEntity, IDeactivatable
//    {
//        // Relacionamentos
//        //public int CompanyId { get; private set; }
//       // public Company Company { get; private set; }
//        public Guid? BranchId { get; private set; }
//        public Branch? Branch { get; private set; }

//        // Controle financeiro
//        public decimal CurrentBalance { get; private set; }
//        private readonly List<CashMovement> _movements = new();
//        public IReadOnlyCollection<CashMovement> Movements => _movements.AsReadOnly();

//        // Status
//        public bool IsActive { get; private set; } = true;

//        protected CashRegister()
//        {
//            // Construtor protegido para ORM
//        }

//        /// <summary>
//        /// Cria um novo caixa associado a uma empresa e opcionalmente a uma filial
//        /// </summary>
//        public CashRegister(Branch? branch = null)
//        {            

//            if (branch != null)
//            {
//                Branch = branch;
//                BranchId = branch.Id;
//            }

//            CurrentBalance = 0; // Saldo inicial zerado
//        }

//        /// <summary>
//        /// Registra uma entrada de valores no caixa (ex: venda)
//        /// </summary>
//        public void RegisterIncome(decimal amount)
//        {
//            if (amount <= 0)
//                throw new ArgumentException("Valor da entrada deve ser positivo", nameof(amount));

//            CurrentBalance += amount;
//            var movement = new CashMovement(this, amount, MovementType.Outbound, DateTime.Now);
//            _movements.Add(movement);
//        }

//        /// <summary>
//        /// Remove uma entrada previamente registrada
//        /// </summary>
//        public void RemoveIncome(decimal amount)
//        {
//            var movement = _movements.FirstOrDefault(m =>
//                m.Amount == amount && m.MovementType == MovementType.Outbound);

//            if (movement == null)
//                throw new InvalidOperationException("Movimento de entrada não encontrado");

//            CurrentBalance -= amount;
//            _movements.Remove(movement);
//        }

//        /// <summary>
//        /// Registra uma saída de valores do caixa (ex: pagamento)
//        /// </summary>
//        public void RegisterOutcome(decimal amount)
//        {
//            if (amount <= 0)
//                throw new ArgumentException("Valor da saída deve ser positivo", nameof(amount));

//            if (CurrentBalance - amount < 0)
//                throw new InvalidOperationException("Saldo insuficiente no caixa");

//            CurrentBalance -= amount;
//            var movement = new CashMovement(this, amount, MovementType.Outbound, DateTime.Now);
//            _movements.Add(movement);
//        }

//        /// <summary>
//        /// Remove uma saída previamente registrada
//        /// </summary>
//        public void RemoveOutcome(decimal amount)
//        {
//            var movement = _movements.FirstOrDefault(m =>
//                m.Amount == amount && m.MovementType == MovementType.Outbound);

//            if (movement == null)
//                throw new InvalidOperationException("Movimento de saída não encontrado");

//            CurrentBalance += amount;
//            _movements.Remove(movement);
//        }

//        /// <summary>
//        /// Desativa o caixa (não permite novas movimentações)
//        /// </summary>
//        public void Deactivate()
//        {
//            if (!IsActive) return;
//            IsActive = false;
//        }

//        /// <summary>
//        /// Reativa o caixa
//        /// </summary>
//        public void Activate()
//        {
//            if (IsActive) return;
//            IsActive = true;
//        }
//    }
//}


using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Representa um caixa ou ponto de venda no sistema
    /// </summary>
    public class CashRegister : BaseEntity
    {
        private bool _isCancelled;

        public string SaleNumber { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public decimal TotalDiscount { get; set; }

        public DateTime SaleDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        // Relacionamentos
        public Guid? BranchId { get; private set; }
        public Branch? Branch { get; private set; }

        // Colaborador responsável pelo caixa
        public Guid? UserId { get; private set; }
        public User? User { get; set; } = null;

        // Cliente
        public Guid? CustomerId { get; private set; }
        public Customer? Customer { get; set; }

        // Controle financeiro
        public decimal CurrentBalance { get; private set; }
        private readonly List<CashMovement> _cashMovements = new();
        public IReadOnlyCollection<CashMovement> CashMovements => _cashMovements.AsReadOnly();

        public virtual ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

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
        // Status
        public bool IsActive { get; private set; } = true;

        protected CashRegister()
        {
            // Construtor protegido para ORM
        }

        /// <summary>
        /// Cria um novo caixa associado a uma filial e usuário responsável
        /// </summary>
        public CashRegister(Branch? branch = null, User? responsibleSupplier = null)
        {
            if (branch != null)
            {
                Branch = branch;
                BranchId = branch.Id;
            }

            if (responsibleSupplier != null)
            {
                User = responsibleSupplier;
                UserId = responsibleSupplier.Id;
            }
            SaleNumber = GenerateSaleNumber();
            CurrentBalance = 0; // Saldo inicial zerado
        }

        public string GenerateSaleNumber()
        {
            return $"SALE-{DateTime.UtcNow:yyyyMMddHHmmssfff}";
        }

        public void AddItem(OrderItem item)
        {
            if (IsCancelled)
                throw new InvalidOperationException("Cannot add items to a cancelled sale.");

            Items.Add(item);
            TotalAmount = CalculateTotalAmount();
        }

        public void RemoveItem(OrderItem item)
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

        public void UpdateSale(DateTime saleDate, Guid customerId, Guid branchId, List<OrderItem> updatedItems)
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

        /// <summary>
        /// Define o usuário responsável pelo caixa
        /// </summary>
        public void SetResponsibleSupplier(User user)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
        }

        public void RegisterIncome(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor da entrada deve ser positivo", nameof(amount));

            CurrentBalance += amount;
            var movement = new CashMovement(this, User, amount, MovementType.Outbound, DateTime.Now);
            _cashMovements.Add(movement);
        }

        /// <summary>
        /// Remove uma entrada previamente registrada
        /// </summary>
        public void RemoveIncome(decimal amount)
        {
            var movement = _cashMovements.FirstOrDefault(m =>
                m.Amount == amount && m.MovementType == MovementType.Outbound);

            if (movement == null)
                throw new InvalidOperationException("Movimento de entrada não encontrado");

            CurrentBalance -= amount;
            _cashMovements.Remove(movement);
        }

        /// <summary>
        /// Registra uma saída de valores do caixa (ex: pagamento)
        /// </summary>
        public void RegisterOutcome(decimal amount)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor da saída deve ser positivo", nameof(amount));

            if (CurrentBalance - amount < 0)
                throw new InvalidOperationException("Saldo insuficiente no caixa");

            CurrentBalance -= amount;
            var movement = new CashMovement(this, User, amount, MovementType.Outbound, DateTime.Now);
            _cashMovements.Add(movement);
        }

        /// <summary>
        /// Remove uma saída previamente registrada
        /// </summary>
        public void RemoveOutcome(decimal amount)
        {
            var movement = _cashMovements.FirstOrDefault(m =>
                m.Amount == amount && m.MovementType == MovementType.Outbound);

            if (movement == null)
                throw new InvalidOperationException("Movimento de saída não encontrado");

            CurrentBalance += amount;
            _cashMovements.Remove(movement);
        }        
    }
}
