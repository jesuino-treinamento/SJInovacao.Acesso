using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe de junção entre FolhaPonto e HoraExtra
    /// </summary>
    public class TimeSheetOvertime : BaseEntity
    {
        public Guid TimeSheetId { get; set; }                  // ID da folha de ponto
        public TimeSheet TimeSheet { get; set; } = null!;     // Navegação para folha de ponto
        public Guid OvertimeId { get; set; }                   // ID da hora extra
        public Overtime Overtime { get; set; } = null!;      // Navegação para hora extra
        public Guid EmployeeId { get; set; }                   // ID do colaborador
        public Employee Employee { get; set; } = null!;       // Navegação para colaborador

        public TimeSheetOvertime()
        {
                
        }
    }
}
