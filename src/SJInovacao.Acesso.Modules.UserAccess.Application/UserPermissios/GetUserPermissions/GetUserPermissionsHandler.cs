using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.UserPermissions.GetUserPermissions
{
    public class GetUserPermissionsHandler : IRequestHandler<GetUserPermissionsQuery, IEnumerable<PermissionDto>>
    {
        private readonly IUserPermissionRepository _repository;

        public GetUserPermissionsHandler(IUserPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(GetUserPermissionsQuery query, CancellationToken ct)
        {

            var permissions = await _repository.GetByUserIdAsync(query.UserId, ct);
            return permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description });
        }
    }
}
