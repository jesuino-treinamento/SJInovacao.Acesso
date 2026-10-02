using SJInovacao.Acesso.Modules.UserAccess.Application.Users;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.DTOs
{
    public class UserGroupDTO
    {
        public Guid UserId { get; set; }
        public Guid GroupId { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navegações (obrigatórias para mapeamento correto)
        public UserResult UserDTO { get; set; } = null!;

        public GroupPermissionsDto GroupDTO { get; set; } = null!;

        //public GroupUsersPermissionDto GroupUserPermissionsDTO { get; set; } = null!;
    }
}
