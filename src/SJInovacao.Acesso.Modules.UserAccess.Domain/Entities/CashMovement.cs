using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um movimento de caixa no sistema
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class CashMovement : BaseEntity, IDeactivatable
    {
        public Guid CashRegisterId { get; set; }                 // ID do caixa relacionado
        public CashRegister CashRegister { get; set; } = null!; // Navegação para o caixa

        // Colaborador que realizou o movimento
        public Guid UserId { get; private set; }
        public User? User { get; private set; } = null!;
        public decimal Amount { get; set; }                     // Valor do movimento
        public MovementType MovementType { get; set; }          // Tipo de movimento (enum)
        public DateTime MovementDate { get; set; }              // Data do movimento
        public bool IsActive { get; private set; } = true;        // Status ativo/inativo

        // Construtor usado pelo EF Core (obrigatório)
        protected CashMovement() { }

        /// <summary>
        /// Construtor para criar um novo movimento de caixa
        /// </summary>
        /// <param name="cashRegister">Caixa relacionado (obrigatório)</param>
        /// <param name="amount">Valor do movimento</param>
        /// <param name="movementType">Tipo de movimento</param>
        /// <param name="movementDate">Data do movimento</param>
        public CashMovement(CashRegister cashRegister, User? user, decimal amount, MovementType movementType, DateTime movementDate)
        {
            CashRegisterId = cashRegister.Id;
            User = user ?? throw new ArgumentNullException(nameof(user));
            UserId = user.Id;
            CashRegister = cashRegister ?? throw new ArgumentNullException(nameof(cashRegister));
            Amount = amount;
            MovementType = movementType;
            MovementDate = movementDate;
        }

        /// <summary>
        /// Desativa o movimento (marca como inativo)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o movimento (marca como ativo)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}