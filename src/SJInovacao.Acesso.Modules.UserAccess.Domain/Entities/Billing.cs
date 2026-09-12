using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    // Classe que representa um faturamento (billing) no domínio da aplicação
    public class Billing : BaseEntity, IDeactivatable
    {
        // Valor total do faturamento
        public decimal Amount { get; set; }

        // Data de emissão do faturamento
        public DateTime IssueDate { get; set; }

        // Data de vencimento do faturamento
        public DateTime DueDate { get; set; }

        // Indica se o faturamento foi pago
        public bool IsPaid { get; set; }

        // ID do cliente associado ao faturamento
        public Guid CustomerId { get; set; }

        // Navegação para o cliente (EF Core)
        public Customer Customer { get; set; } = null!;

        // Coleção de contas a receber associadas a este faturamento
        public ICollection<AccountReceivable> AccountReceivables { get; set; } = new List<AccountReceivable>();

        // Indica se o faturamento está ativo (implementação de IDeactivatable)
        public bool IsActive { get; private set; }

        // Construtor protegido para uso do EF Core
        protected Billing() { }

        // Construtor principal para criar um novo faturamento
        public Billing(decimal amount, DateTime issueDate, DateTime dueDate, Guid clientId)
        {
            Amount = amount;
            IssueDate = issueDate;
            DueDate = dueDate;
            IsPaid = false; // Por padrão, faturamento é criado como não pago
            CustomerId = clientId;
            IsActive = true; // Por padrão, faturamento é criado como ativo
        }

        // Método para desativar o faturamento
        public void Deactivate()
        {
            if (!IsActive) return; // Se já estiver inativo, não faz nada
            IsActive = false;
        }

        // Método para ativar o faturamento
        public void Activate()
        {
            if (IsActive) return; // Se já estiver ativo, não faz nada
            IsActive = true;
        }
    }
}