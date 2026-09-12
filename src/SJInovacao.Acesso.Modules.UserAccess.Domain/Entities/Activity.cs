using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa uma atividade/função do colaborador
    /// </summary>
    public class Activity : BaseEntity, IDeactivatable
    {
        public string Position { get; set; } = string.Empty;       // Cargo
        public string Function { get; set; } = string.Empty;      // Função
        public string Workplace { get; set; } = string.Empty;     // Local de trabalho
        public int Workload { get; set; }                         // Carga horária
        public Guid EmployeeId { get; set; }                       // ID do colaborador
        public Employee Employee { get; set; } = null!;           // Navegação para colaborador
        public bool IsActive { get; private set; }                  // Status ativo/inativo

        protected Activity() { }

        public Activity(string position, string function, string workplace, int workload)
        {
            Position = position ?? throw new ArgumentNullException(nameof(position));
            Function = function ?? throw new ArgumentNullException(nameof(function));
            Workplace = workplace ?? throw new ArgumentNullException(nameof(workplace));
            Workload = workload >= 0 ? workload :
                throw new ArgumentOutOfRangeException(nameof(workload), "A carga horária não pode ser negativa.");
            IsActive = true;
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
