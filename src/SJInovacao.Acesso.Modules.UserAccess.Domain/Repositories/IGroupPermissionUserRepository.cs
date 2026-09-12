using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories
{
    public interface IGroupPermissionUserRepository
    {
        Task DeleteAsync(Guid groupId, Guid permissionId, CancellationToken ct);
    }    
}
