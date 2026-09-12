using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um movimento de estoque no sistema
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class Inventory : BaseEntity, IDeactivatable
    {
        public Guid ProductId { get; private set; }                  // ID do produto
        public Product Product { get; private set; } = null!;       // Navegação para o produto
        public int Quantity { get; private set; }                   // Quantidade movimentada
        public MovementType MovementType { get; private set; }      // Tipo de movimento (Entrada/Saída)
        public DateTime MovementDate { get; private set; }           // Data do movimento
        public bool IsActive { get; private set; } = true;            // Status ativo/inativo

        /// <summary>
        /// Construtor protegido para EF Core
        /// </summary>
        protected Inventory() { }

        /// <summary>
        /// Cria um novo movimento de estoque
        /// </summary>
        /// <param name="productId">ID do produto</param>
        /// <param name="quantity">Quantidade (não pode ser zero)</param>
        /// <param name="movementType">Tipo de movimento</param>
        /// <exception cref="ArgumentException">Se a quantidade for zero</exception>
        public Inventory(Guid productId, int quantity, MovementType movementType)
        {
            if (quantity == 0)
                throw new ArgumentException("A quantidade não pode ser zero.", nameof(quantity));

            ProductId = productId;
            Quantity = quantity;
            MovementType = movementType;
            MovementDate = DateTime.UtcNow;
        }

        /// <summary>
        /// Calcula o novo saldo de estoque após este movimento
        /// </summary>
        /// <param name="currentStock">Quantidade atual em estoque</param>
        /// <returns>Novo saldo de estoque</returns>
        public int CalculateNewStock(int currentStock)
        {
            return MovementType == MovementType.Inbound
                ? currentStock + Quantity
                : currentStock - Quantity;
        }

        /// <summary>
        /// Desativa o movimento de estoque
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o movimento de estoque
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}