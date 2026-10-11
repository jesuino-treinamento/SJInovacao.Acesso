namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Common.Projections
{
    public record UserGroupSummary(
        Guid UserId,
        string UserName,
        Guid GroupId,
        string GroupName,
        bool UserIsActive);

    public record UserGroupWithPermissionsSummary(
        Guid UserId,
        string UserName,
        Guid GroupId,
        string GroupName,
        bool UserIsActive,
        List<GroupUserPermissionSummary> Permissions);

    public record GroupUserPermissionSummary(
        Guid Id,
        string Name,
        string Description,
        bool IsActive,
        DateTime CreatedAt,
        DateTime? UpdatedAt);
}