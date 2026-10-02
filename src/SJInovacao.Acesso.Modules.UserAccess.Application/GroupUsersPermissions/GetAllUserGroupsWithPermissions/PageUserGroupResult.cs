namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupUsersPermissions.GetAllUserGroupsWithPermissions
{
    public class PageUserGroupResult
    {
        public IEnumerable<GroupUsersPermissionResult?> Data { get; set; } = new List<GroupUsersPermissionResult?>();
        public int TotalItems { get; set; } = 0;
        public int CurrentPage { get; set; } = 0;
        public int TotalPages { get; set; } = 0;
    }
}
