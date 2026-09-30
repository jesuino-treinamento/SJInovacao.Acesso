using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.DeleteGroupAccess
{
    public class DeleteGroupAccessCommand : IRequest<DeleteGroupAccessResult>
    {
        public Guid GroupId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
