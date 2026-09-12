using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa uma folha de ponto do colaborador
    /// </summary>
    public class TimeSheet : BaseEntity, IDeactivatable
    {
        public DateTime Date { get; set; }                    // Data do registro
        public TimeSpan EntryTime { get; set; }               // Hora de entrada
        public TimeSpan ExitTime { get; set; }                // Hora de saída
        public TimeSpan BreakTime { get; set; }               // Intervalo
        public decimal TotalHours { get; set; }               // Total de horas
        public Guid EmployeeId { get; set; }                   // ID do colaborador
        public Employee Employee { get; set; } = null!;       // Navegação para colaborador
        public ICollection<Overtime> Overtimes { get; set; } = new List<Overtime>(); // Horas extras
        public List<TimeSheetOvertime> TimeSheetOvertimes { get; set; } = new(); // Relação com horas extras
        public JustificationType? JustificationReason { get; set; } // Motivo de justificativa
        public string? Observation { get; set; }              // Observações
        public bool IsActive { get; private set; } = true;      // Status ativo/inativo

        public TimeSheet()
        {
                
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
