using System.ComponentModel;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Enums
{
    /// <summary>
    /// Enum que representa os tipos de movimentação no sistema
    /// </summary>
    public enum MovementType
    {
        /// <summary>
        /// Entrada de itens no estoque
        /// </summary>
        [Description("Entrada no estoque")]
        Inbound,

        /// <summary>
        /// Saída de itens do estoque
        /// </summary>
        [Description("Saída do estoque")]
        Outbound,

        /// <summary>
        /// Movimentação financeira (receita)
        /// </summary>
        [Description("Receita")]
        Income
    }
}