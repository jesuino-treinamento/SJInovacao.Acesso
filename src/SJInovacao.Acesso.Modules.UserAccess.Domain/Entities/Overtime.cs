using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa horas extras de um colaborador
    /// </summary>
    public class Overtime : BaseEntity, IDeactivatable
    {
        public Guid EmployeeId { get; private set; }                     // ID do colaborador
        public Employee Employee { get; private set; } = null!;         // Navegação para colaborador

        public DateTime OvertimeDate { get; private set; }              // Data da hora extra
        public decimal Hours { get; private set; }                      // Quantidade de horas
        public bool Approved { get; private set; }                      // Se foi aprovada
        public bool IsActive { get; private set; } = true;              // Status ativo/inativo

        private readonly List<TimeSheetOvertime> _timeSheetOvertimes = new();
        public IReadOnlyCollection<TimeSheetOvertime> TimeSheetOvertimes => _timeSheetOvertimes.AsReadOnly();

        private readonly List<TimeSheet> _timeSheets = new();
        public IReadOnlyCollection<TimeSheet> TimeSheets => _timeSheets.AsReadOnly();

        /// <summary>
        /// Construtor protegido para EF
        /// </summary>
        protected Overtime() { }

        /// <summary>
        /// Construtor para criar nova hora extra
        /// </summary>
        public Overtime(Guid employeeId, DateTime overtimeDate, decimal hours, bool approved)
        {
            if (hours <= 0)
                throw new ArgumentException("Horas devem ser maiores que zero", nameof(hours));

            EmployeeId = employeeId;
            OvertimeDate = overtimeDate;
            Hours = hours;
            Approved = approved;
            IsActive = true;
        }

        /// <summary>
        /// Marca a hora extra como aprovada
        /// </summary>
        public void Approve()
        {
            Approved = true;
        }

        /// <summary>
        /// Desmarca a aprovação da hora extra
        /// </summary>
        public void RevokeApproval()
        {
            Approved = false;
        }

        /// <summary>
        /// Desativa a hora extra
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa a hora extra
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
