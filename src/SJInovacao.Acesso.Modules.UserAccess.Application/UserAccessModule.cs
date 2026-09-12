using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.Contracts;

namespace SJInovacao.Acesso.Modules.UserAccess.Application
{
    public class UserAccessModule : IUserAccessModule
    {
        private readonly IMediator _mediator;

        public UserAccessModule(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<TResult> ExecuteCommandAsync<TResult>(IRequest<TResult> command, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(command, cancellationToken);
        }

        public async Task<TResult> ExecuteQueryAsync<TResult>(IRequest<TResult> query, CancellationToken cancellationToken = default)
        {
            return await _mediator.Send(query, cancellationToken);
        }
    }

}
