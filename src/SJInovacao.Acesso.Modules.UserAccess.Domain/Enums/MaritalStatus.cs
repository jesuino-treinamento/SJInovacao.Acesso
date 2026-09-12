using System.ComponentModel;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Enums
{
    /// <summary>
    /// Enum que representa os estados civis de uma pessoa
    /// </summary>
    public enum MaritalStatus
    {
        /// <summary>
        /// Pessoa solteira
        /// </summary>
        [Description("Solteiro(a)")]
        Solteira, // Single,

        /// <summary>
        /// Pessoa casada
        /// </summary>
        [Description("Casado(a)")]
        Casado, //Married,

        /// <summary>
        /// Pessoa divorciada
        /// </summary>
        [Description("Divorciado(a)")]
        Divorciada, //Divorced,

        /// <summary>
        /// Pessoa viúva
        /// </summary>
        [Description("Viúvo(a)")]
        Viúva, //Widowed,

        /// <summary>
        /// Pessoa separada
        /// </summary>
        [Description("Separado(a)")]
        Separada, //Separated,

        /// <summary>
        /// União estável
        /// </summary>
        [Description("União Estável")]
        Uniao_Estavel, //CivilUnion
    }
}
