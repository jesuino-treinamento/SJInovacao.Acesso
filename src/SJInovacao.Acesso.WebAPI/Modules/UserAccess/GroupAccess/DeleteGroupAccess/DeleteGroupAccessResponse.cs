using SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.DeleteGroupAccess;

namespace SJInovacao.Acesso.WebAPI.Modules.UserAccess.GroupAccess.DeleteGroupAccess
{
    public class DeleteGroupAccessResponse
    {        
        public Guid GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty; // Novo campo       
    }
}
