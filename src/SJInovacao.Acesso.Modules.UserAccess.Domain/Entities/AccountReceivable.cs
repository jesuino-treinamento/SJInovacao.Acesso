using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa uma conta a receber no sistema
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class AccountReceivable : BaseEntity, IDeactivatable
    {
        public decimal Amount { get; private set; }               // Valor da conta
        public DateTime DueDate { get; private set; }            // Data de vencimento
        public DateTime? PaymentDate { get; private set; }       // Data de pagamento (null se não paga)
        public bool IsPaid { get; private set; }                 // Indica se a conta foi paga
        public Guid BillingId { get; private set; }               // ID do faturamento relacionado
        public Billing Billing { get; private set; } = null!;    // Navegação para o faturamento
        public bool IsActive { get; private set; } = true;         // Status ativo/inativo

        public AccountReceivable()
        {
                
        }

        /// <summary>
        /// Construtor para criar uma nova conta a receber
        /// </summary>
        /// <param name="amount">Valor da conta (deve ser positivo)</param>
        /// <param name="dueDate">Data de vencimento (deve ser futura)</param>
        /// <param name="billingId">ID do faturamento relacionado</param>
        /// <exception cref="ArgumentException">Quando parâmetros são inválidos</exception>
        public AccountReceivable(decimal amount, DateTime dueDate, Guid billingId)
        {
            if (amount <= 0)
                throw new ArgumentException("O valor da conta deve ser positivo", nameof(amount));

            if (dueDate.Date < DateTime.Today)
                throw new ArgumentException("A data de vencimento deve ser futura", nameof(dueDate));

            Amount = amount;
            DueDate = dueDate;
            BillingId = billingId;
            IsPaid = false;
        }

        /// <summary>
        /// Registra o pagamento da conta
        /// </summary>
        /// <exception cref="InvalidOperationException">Se a conta já estiver paga</exception>
        public void ReceivePayment()
        {
            if (IsPaid)
                throw new InvalidOperationException("Esta conta já foi paga");

            IsPaid = true;
            PaymentDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Verifica se a conta está vencida
        /// </summary>
        public bool IsOverdue => !IsPaid && DueDate.Date < DateTime.UtcNow;

        /// <summary>
        /// Calcula dias de atraso (retorna 0 se não estiver vencida)
        /// </summary>
        public int DaysOverdue => IsOverdue ? (DateTime.UtcNow - DueDate.Date).Days : 0;

        /// <summary>
        /// Desativa a conta a receber
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa a conta a receber
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}