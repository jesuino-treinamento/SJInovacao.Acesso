using MediatR;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.Contracts
{
    public interface IUserAccessModule
    {
        Task<TResult> ExecuteCommandAsync<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default);
        Task<TResult> ExecuteQueryAsync<TResult>(IRequest<TResult> query, CancellationToken cancellationToken = default);
    }
}
