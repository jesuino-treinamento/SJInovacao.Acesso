using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Contracts
{
    public interface IQuery<out TResult> : IRequest<TResult>
    {
    }
}
