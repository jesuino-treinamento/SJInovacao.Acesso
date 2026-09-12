using SJInovacao.Acesso.Modules.UserAccess.Application.Contracts;
using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Configuration.Queries
{
    public interface IQueryHandler<in TQuery, TResult> :
        IRequestHandler<TQuery, TResult>
        where TQuery : IQuery<TResult>
    {
    }
}
