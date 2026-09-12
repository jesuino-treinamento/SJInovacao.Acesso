using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa uma conta a pagar no sistema
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class AccountPayable : BaseEntity, IDeactivatable
    {
        public decimal Amount { get; private set; }               // Valor da conta
        public DateTime DueDate { get; private set; }            // Data de vencimento
        public DateTime? PaymentDate { get; private set; }       // Data de pagamento (null se não paga)
        public bool IsPaid { get; private set; }                 // Indica se a conta foi paga
        public Guid SupplierId { get; private set; }              // ID do fornecedor
        public Supplier Supplier { get; private set; } = null!;  // Navegação para o fornecedor
        public bool IsActive { get; private set; } = true;         // Status ativo/inativo

        public AccountPayable()
        {
                
        }

        /// <summary>
        /// Construtor para criar uma nova conta a pagar
        /// </summary>
        /// <param name="amount">Valor da conta (deve ser positivo)</param>
        /// <param name="dueDate">Data de vencimento (deve ser futura)</param>
        /// <param name="supplierId">ID do fornecedor</param>
        /// <exception cref="ArgumentException">Quando parâmetros são inválidos</exception>
        public AccountPayable(decimal amount, DateTime dueDate, Guid supplierId)
        {
            if (amount <= 0)
                throw new ArgumentException("Valor deve ser positivo", nameof(amount));

            if (dueDate.Date < DateTime.UtcNow)
                throw new ArgumentException("Data de vencimento deve ser futura", nameof(dueDate));

            Amount = amount;
            DueDate = dueDate;
            SupplierId = supplierId;
            IsPaid = false;
        }

        /// <summary>
        /// Registra o pagamento da conta
        /// </summary>
        /// <exception cref="InvalidOperationException">Se a conta já estiver paga</exception>
        public void Pay()
        {
            if (IsPaid)
                throw new InvalidOperationException("Conta já foi paga");

            IsPaid = true;
            PaymentDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Marca a conta como inativa
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Marca a conta como ativa
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

        /// <summary>
        /// Verifica se a conta está vencida
        /// </summary>
        public bool IsOverdue => !IsPaid && DueDate.Date < DateTime.UtcNow;
    }
}