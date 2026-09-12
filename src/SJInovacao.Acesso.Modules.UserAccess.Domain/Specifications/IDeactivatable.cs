namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications
{
    /// <summary>
    /// Interface que define um contrato para entidades que podem ser ativadas/desativadas
    /// </summary>
    public interface IDeactivatable
    {
        /// <summary>Indica se a entidade está ativa no sistema</summary>
        bool IsActive { get; }

        /// <summary>Ativa a entidade no sistema</summary>
        void Activate();

        /// <summary>Desativa a entidade no sistema</summary>
        void Deactivate();
    }
}
