using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um pagamento de salário ao colaborador
    /// </summary>
    public class EmployeePayment : BaseEntity, IDeactivatable
    {
        public decimal AmountPaid { get; set; }               // Valor pago
        public DateTime PaymentDate { get; set; }             // Data do pagamento
        public Guid EmployeeId { get; set; }                   // ID do colaborador
        public Employee Employee { get; set; } = null!;       // Navegação para colaborador
        public Guid SalaryId { get; set; }                     // ID do salário
        public Salary Salary { get; set; } = null!;           // Navegação para salário
        public decimal Overtime50 { get; private set; }       // Horas extras 50%
        public decimal Overtime70 { get; private set; }       // Horas extras 70%
        public decimal Overtime100 { get; private set; }      // Horas extras 100%
        public bool IsActive { get; private set; } = true;      // Status ativo/inativo

        protected EmployeePayment() { }

        public EmployeePayment(decimal amountPaid, DateTime paymentDate, Guid employeeId,
                             Guid salaryId, decimal overtime50, decimal overtime70, decimal overtime100)
        {
            AmountPaid = amountPaid;
            PaymentDate = paymentDate;
            EmployeeId = employeeId;
            SalaryId = salaryId;
            Overtime50 = overtime50;
            Overtime70 = overtime70;
            Overtime100 = overtime100;
        }

        /// <summary>
        /// Calcula o total do pagamento incluindo horas extras
        /// </summary>
        /// <param name="monthlyWorkHours">Horas trabalhadas no mês (padrão 220)</param>
        /// <returns>Valor total do pagamento</returns>
        public decimal CalculateTotalPayment(decimal monthlyWorkHours = 220)
        {
            decimal hourValue = Salary.CurrentSalary / monthlyWorkHours;
            decimal extra50 = Overtime50 * hourValue * 1.5m;
            decimal extra70 = Overtime70 * hourValue * 1.7m;
            decimal extra100 = Overtime100 * hourValue * 2.0m;

            return Salary.CurrentSalary + extra50 + extra70 + extra100;
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
