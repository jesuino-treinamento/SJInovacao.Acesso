using AutoMapper;
using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Repositories;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUGWithPermissionsPaginatedHandler
        : IRequestHandler<GetAllUGWithPermissionsPaginatedQuery, PaginatedList<GroupUsersPermissionResult>>
    {
        private readonly IGroupPermissionUserRepository _repository;
        private readonly IMapper _mapper;

        public GetAllUGWithPermissionsPaginatedHandler(
            IGroupPermissionUserRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaginatedList<GroupUsersPermissionResult>> Handle(
            GetAllUGWithPermissionsPaginatedQuery request,
            CancellationToken cancellationToken)
        {
            var page = await _repository.GetAllPaginatedAsync(
                request.Page, request.Size, request.Order, cancellationToken);

            if (page.Items.Count == 0)
            {
                return new PaginatedList<GroupUsersPermissionResult>(
                    new List<GroupUsersPermissionResult>(),
                    page.TotalCount, page.PageNumber, page.PageSize);
            }

            var userIds = page.Items.Select(p => p.UserId).Distinct().ToList();
            var groupIds = page.Items.Select(p => p.GroupId).Distinct().ToList();

            var permissions = await _repository.GetPermissionsForPageAsync(
                userIds, groupIds, cancellationToken);

            var results = page.Items.Select(ug => new GroupUsersPermissionResult
            {
                UserId = ug.UserId,
                UserName = ug.UserName,
                GroupId = ug.GroupId,
                GroupName = ug.GroupName,
                UserIsActive = ug.UserIsActive,
                Permissions = permissions.TryGetValue((ug.UserId, ug.GroupId), out var list)
                    ? _mapper.Map<List<PermissionDto>>(list)
                    : new List<PermissionDto>()
            }).ToList();

            return new PaginatedList<GroupUsersPermissionResult>(
                results, page.TotalCount, page.PageNumber, page.PageSize);
        }
    }
}