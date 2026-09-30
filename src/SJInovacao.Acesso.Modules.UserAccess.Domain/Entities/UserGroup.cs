using System.Text.RegularExpressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class UserGroup
    {
        public Guid UserId { get; set; }     
        public Guid GroupId { get; set; }      
        public bool IsActive { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegações (obrigatórias para mapeamento correto)
        public User User { get; set; } = null!;

        public GroupPermission Group { get; set; } = null!;
    }
}
