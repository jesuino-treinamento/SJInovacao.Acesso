namespace SJInovacao.Acesso.Modules.UserAccess.Application.GroupAccess.RemoveGroupAccess
{
    public class RemoveGroupAccessResult
    {
        public Guid GroupId { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime UpdatedAt { get; set; }
    }
}
