namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.UpdateGroupAccess
{
    public class RemoveGroupAccessRequest
    {
        public Guid UserId { get; set; }
        public Guid PermissionId { get; set; }
    }
}
