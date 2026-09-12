using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    public class Permission : BaseEntity, IDeactivatable
    {
        public Permission() { }

        public Permission(string name, string description)
        {
            Name = name;
            Description = description;
            CreatedAt = DateTime.UtcNow;
        }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Usuários com permissão direta
        public ICollection<User> Users { get; set; } = new List<User>();

        // Grupos que possuem esta permissão
        public ICollection<GroupPermission> Groups { get; set; } = new List<GroupPermission>();

        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o endereço (marca como ativo)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }

    }
}
