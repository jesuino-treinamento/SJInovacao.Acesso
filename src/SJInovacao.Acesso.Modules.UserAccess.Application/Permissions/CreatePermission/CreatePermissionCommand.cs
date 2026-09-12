using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Permissions.CreatePermission
{
    public class CreatePermissionCommand : IRequest<PermissionDto>
    {
        public Guid Id { get; internal set; }
        public string Name { get; set; } = string.Empty; 
        public string Description { get; set; } = string.Empty;
    }
}
