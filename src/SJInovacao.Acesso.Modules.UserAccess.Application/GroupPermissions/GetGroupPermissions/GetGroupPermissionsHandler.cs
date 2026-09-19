using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetGroupPermissions
{
    public class GetGroupPermissionsHandler : IRequestHandler<GetGroupPermissionsQuery, IEnumerable<PermissionDto>>
    {
        private readonly IGroupPermissionRepository _repository;

        public GetGroupPermissionsHandler(IGroupPermissionRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<PermissionDto>> Handle(GetGroupPermissionsQuery query, CancellationToken ct)
        {

            var permissions = await _repository.GetByGroupIdAsync(query.GroupId, ct);
            return permissions.Select(p => new PermissionDto { Id = p.Id, Name = p.Name, Description = p.Description });
        }
    }
}
