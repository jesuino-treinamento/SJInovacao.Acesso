using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um colaborador/funcionário da empresa
    /// Herda de Pessoa e contém todas as informações trabalhistas
    /// </summary>
    public class Employee : BaseEntity, IDeactivatable
    {
        private static int _lastRegistrationNumber = 10000;  // Valor inicial para matrícula

        public Guid UserId { get; set; }                    // ID da User (herdada)
        public Guid? BranchId { get; private set; }           // ID da filial
        public Branch? Branch { get; private set; }          // Navegação para filial
        public string RegistrationNumber { get; private set; } = string.Empty;  // Matrícula
        public byte[] Photo { get; private set; } = Array.Empty<byte>();       // Foto
        public string RG { get; private set; } = string.Empty;                 // RG
        public DateTime BirthDate { get; private set; }      // Data de nascimento
        public DateTime HireDate { get; private set; }       // Data de admissão
        public DateTime? TerminationDate { get; private set; } // Data de demissão
        public MaritalStatus? MaritalStatus { get; private set; } // Estado civil
        public EducationLevel? EducationLevel { get; private set; } // Formação escolar
        public string FatherName { get; private set; } = string.Empty; // Nome do pai
        public string MotherName { get; private set; } = string.Empty; // Nome da mãe
        public string VoterId { get; private set; } = string.Empty;    // Título de eleitor
        public string MilitaryId { get; private set; } = string.Empty; // Reservista
        public string PIS { get; private set; } = string.Empty;       // PIS/PASEP
        public string PASEP { get; private set; } = string.Empty;     // PASEP
        public string WorkCard { get; private set; } = string.Empty;   // CTPS

        // Coleções relacionadas
        private readonly List<Activity> _activities = new();
        public IReadOnlyCollection<Activity> Activities => _activities.AsReadOnly();

        private readonly List<Salary> _salaries = new();
        public IReadOnlyCollection<Salary> Salaries => _salaries.AsReadOnly();

        private readonly List<TimeSheet> _timeSheets = new();
        public IReadOnlyCollection<TimeSheet> TimeSheets => _timeSheets.AsReadOnly();

        private readonly List<Overtime> _overtimes = new();
        public IReadOnlyCollection<Overtime> Overtimes => _overtimes.AsReadOnly();

        public TimeBank TimeBank { get; private set; } = null!;  // Banco de horas

        private readonly List<Vacation> _vacations = new();
        public IReadOnlyCollection<Vacation> Vacations => _vacations.AsReadOnly();

        private readonly List<TimeSheetOvertime> _timeSheetOvertimes = new();
        public IReadOnlyCollection<TimeSheetOvertime> TimeSheetOvertimes => _timeSheetOvertimes.AsReadOnly();

        private readonly List<EmployeePayment> _employeePayments = new();
        public IReadOnlyCollection<EmployeePayment> EmployeePayments => _employeePayments.AsReadOnly();

        public User? User { get; set; }
        public Document Document { get; protected set; } = null!;
        public DateTime DateAdmission { get; set; }
        public DateTime DateDismissal { get; set; }

        public bool IsActive { get; private set; } = true;

        /// <summary>
        /// Construtor protegido para EF Core
        /// </summary>
        protected Employee() { }

        /// <summary>
        /// Cria um novo colaborador
        /// </summary>
        /// <param name="name">Nome completo</param>
        /// <param name="personType">Tipo de pessoa (Física/Jurídica)</param>
        /// <param name="document">Documento (CPF/CNPJ)</param>
        /// <param name="email">E-mail</param>
        /// <param name="birthDate">Data de nascimento</param>
        /// <param name="hireDate">Data de admissão</param>
        /// <param name="company">Empresa contratante</param>
        /// <param name="branch">Filial (opcional)</param>
        public Employee(string name, PersonType personType, Document document, string email,
                       DateTime birthDate, DateTime hireDate, Branch? branch = null)
            //: base(name, personType, document, email)
        {
            BirthDate = birthDate;
            HireDate = hireDate;

            if (branch != null)
            {
                Branch = branch;
                BranchId = branch.Id;
            }

            TimeBank = new TimeBank(this);
            DateAdmission =  DateTime.UtcNow;
        }

        // Métodos para adicionar itens às coleções
        public void AddActivity(Activity activity) => _activities.Add(activity);
        public void AddSalary(Salary salary) => _salaries.Add(salary);
        public void AddOvertime(Overtime overtime) => _overtimes.Add(overtime);
        public void AddTimeSheet(TimeSheet timeSheet) => _timeSheets.Add(timeSheet);
        public void AddPayment(EmployeePayment payment) => _employeePayments.Add(payment);

        // Métodos para remover itens das coleções
        public void RemoveActivity(Activity activity) => _activities.Remove(activity);
        public void RemoveSalary(Salary salary) => _salaries.Remove(salary);
        public void RemoveOvertime(Overtime overtime) => _overtimes.Remove(overtime);
        public void RemoveTimeSheet(TimeSheet timeSheet) => _timeSheets.Remove(timeSheet);
        public void RemovePayment(EmployeePayment payment) => _employeePayments.Remove(payment);

        /// <summary>
        /// Atualiza a foto do colaborador
        /// </summary>
        /// <param name="newPhoto">Nova foto em byte array</param>
        public void UpdatePhoto(byte[] newPhoto) => Photo = newPhoto;

        /// <summary>
        /// Gera a matrícula do colaborador baseada no total de registros + 1
        /// </summary>
        /// <param name="existingCount">Quantidade total de colaboradores existentes</param>
        public void GenerateRegistrationNumber(int existingCount)
        {
            RegistrationNumber = $"EMP{(existingCount + 1):D6}";
        }

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
            DateDismissal = DateTime.UtcNow;
        }

        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
