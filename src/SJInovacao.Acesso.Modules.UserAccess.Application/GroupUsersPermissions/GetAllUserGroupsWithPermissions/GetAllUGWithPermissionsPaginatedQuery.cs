using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUGWithPermissionsPaginatedQuery : IRequest<PaginatedList<GroupUsersPermissionResult>>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "UserName, GroupName";
        public bool OnlyActiveUsers { get; set; } = false;
        public string? PermissionNameFilter { get; set; }
        public int? MaxResults { get; set; }
    }
}
