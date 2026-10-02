using MediatR;
using SJInovacao.Acesso.Common.Validation;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Pagination;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class GetAllUGWithPermissionsPaginatedQuery : IRequest<PaginatedList<GroupUsersPermissionResult>>
    {
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
        public string Order { get; set; } = "UserName, GroupName";

        /// <summary>
        /// Se definido como true, retorna apenas usuários com Status = Active.
        /// </summary>
        public bool OnlyActiveUsers { get; set; } = false;

        /// <summary>
        /// Permite filtrar por nome de permissão (ex.: "Admin", "Financeiro").
        /// </summary>
        public string? PermissionNameFilter { get; set; }

        /// <summary>
        /// Permite limitar o número máximo de usuários retornados.
        /// </summary>
        public int? MaxResults { get; set; }

        //public ValidationResultDetail Validate()
        //{
        //    return new ValidationResultDetail(new GetAllUsersQueryValidator().Validate(this));
        //}
    }
}
