using System.ComponentModel;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Enums
{
    public enum PhoneType
    {
        None = 0,
        [Description("Residencial")]
        Residencial,

        [Description("Celular")]
        Celular,

        [Description("Empresa")]
        Empresa
    }
}
