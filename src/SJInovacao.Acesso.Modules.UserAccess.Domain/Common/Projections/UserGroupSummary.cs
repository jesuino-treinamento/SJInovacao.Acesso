namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Projections
{
    /// <summary>
    /// Visão resumida de um vínculo UserGroup, usada em listagens paginadas.
    /// Não carrega a entidade completa (evita Password, RefreshToken, etc.).
    /// </summary>
    public record UserGroupSummary(
        Guid UserId,
        string UserName,
        Guid GroupId,
        string GroupName,
        bool UserIsActive);

    /// <summary>
    /// Visão resumida de um vínculo UserGroup incluindo suas permissões ativas.
    /// </summary>
    public record UserGroupWithPermissionsSummary(
        Guid UserId,
        string UserName,
        Guid GroupId,
        string GroupName,
        bool UserIsActive,
        List<GroupUserPermissionSummary> Permissions);

    /// <summary>
    /// Visão resumida de uma permissão associada a um UserGroup.
    /// </summary>
    public record GroupUserPermissionSummary(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}