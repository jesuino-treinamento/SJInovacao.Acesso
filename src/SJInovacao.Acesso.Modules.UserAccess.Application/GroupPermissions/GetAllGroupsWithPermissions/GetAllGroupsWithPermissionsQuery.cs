using MediatR;
using SJInovacao.Acesso.Modules.UserAccess.Application.DTOs;

namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupPermissions.GetAllGroupsWithPermissions
{
    /// <summary>
    /// Consulta para obter todos os usuários com suas permissões.
    /// Pode incluir filtros opcionais, como status ou tipo de permissão.
    /// </summary>
    public class GetAllGroupsWithPermissionsQuery : IRequest<IEnumerable<GroupPermissionsDto>>
    {
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
    }
}
