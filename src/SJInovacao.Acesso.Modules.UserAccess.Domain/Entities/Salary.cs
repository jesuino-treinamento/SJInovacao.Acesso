using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa o salário do colaborador
    /// </summary>
    public class Salary : BaseEntity, IDeactivatable
    {
        public decimal BaseSalary { get; set; }           // Salário base
        public decimal CurrentSalary { get; set; }        // Salário atual
        public Guid EmployeeId { get; set; }               // ID do colaborador
        public Employee Employee { get; set; } = null!;   // Navegação para colaborador
        public bool IsActive { get; private set; } = true;  // Status ativo/inativo
        private readonly List<EmployeePayment> _employeePayments = new();
        public IReadOnlyCollection<EmployeePayment> EmployeePayments => _employeePayments.AsReadOnly();

        protected Salary() { }

        public Salary(decimal baseSalary, decimal currentSalary, Guid employeeId)
        {
            if (baseSalary < 0)
                throw new ArgumentException("O salário base não pode ser negativo.", nameof(baseSalary));
            if (currentSalary < 0)
                throw new ArgumentException("O salário atual não pode ser negativo.", nameof(currentSalary));

            BaseSalary = baseSalary;
            CurrentSalary = currentSalary;
            EmployeeId = employeeId;
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
