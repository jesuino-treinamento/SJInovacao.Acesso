using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.RemoveGroupAccess
{
    public class RemoveGroupAccessHandler : IRequestHandler<RemoveGroupAccessCommand, RemoveGroupAccessResult>
    {
        private readonly IGroupPermissionRepository _repository;
        private readonly IMapper _mapper;

        public RemoveGroupAccessHandler(IGroupPermissionRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RemoveGroupAccessResult> Handle(RemoveGroupAccessCommand command, CancellationToken ct)
        {
            await _repository.RemoveAsync(command.GroupId, ct);

            return new RemoveGroupAccessResult
            {
                GroupId = command.GroupId,
                IsActive = false,
                UpdatedAt = DateTime.UtcNow
            };
        }
    }
}
