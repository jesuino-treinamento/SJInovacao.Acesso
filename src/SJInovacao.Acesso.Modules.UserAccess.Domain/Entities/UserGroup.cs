using System.Text.RegularExpressions;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class UserGroup
    {
        public Guid UserId { get; set; }        // tipo da chave de User
        public Guid GroupId { get; set; }       // tipo da chave de Group
        public bool IsActive { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegações (obrigatórias para mapeamento correto)
        public User User { get; set; } = null!;

        public GroupPermission Group { get; set; } = null!;

    }
}
