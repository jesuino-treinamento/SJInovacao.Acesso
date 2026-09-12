using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um período de férias de um colaborador
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class Vacation : BaseEntity, IDeactivatable
    {
        public Guid EmployeeId { get; set; }                   // ID do colaborador
        public Employee Employee { get; set; } = null!;       // Navegação para o colaborador
        public DateTime StartDate { get; set; }               // Data de início das férias
        public DateTime EndDate { get; set; }                 // Data de término das férias
        public bool Approved { get; set; }                    // Indica se as férias foram aprovadas
        public bool IsActive { get; private set; } = true;      // Status ativo/inativo

        public Vacation()
        {
                
        }

        /// <summary>
        /// Valida se as datas de férias são válidas
        /// </summary>
        private void ValidateDates()
        {
            if (StartDate >= EndDate)
                throw new InvalidOperationException("A data de início deve ser anterior à data de término");

            if ((EndDate - StartDate).TotalDays < 5)
                throw new InvalidOperationException("O período mínimo de férias é de 5 dias");
        }

        /// <summary>
        /// Desativa o registro de férias
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o registro de férias
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}