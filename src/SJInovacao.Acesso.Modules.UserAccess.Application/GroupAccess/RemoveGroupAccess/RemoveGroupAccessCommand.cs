using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.RemoveGroupAccess
{
    public class RemoveGroupAccessCommand : IRequest<RemoveGroupAccessResult>
    {
        public Guid GroupId { get; set; }
    }
}
