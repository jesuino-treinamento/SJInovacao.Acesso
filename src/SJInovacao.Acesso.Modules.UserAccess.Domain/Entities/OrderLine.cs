using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa uma linha de pedido no sistema
    /// Implementa IDeactivatable para permitir desativação/ativação
    /// </summary>
    public class OrderLine : BaseEntity, IDeactivatable
    {
        public Guid SupplierOrderId { get; private set; }              // FK explícita
        public SupplierOrder SupplierOrder { get; private set; } = null!; // Navegação para o pedido

        public Guid ProductId { get; private set; }                   // ID do produto
        public Product Product { get; private set; } = null!;        // Navegação para o produto

        public int Quantity { get; private set; }                    // Quantidade do item
        public decimal UnitPrice { get; private set; }               // Preço unitário do item
        public bool IsActive { get; private set; }                   // Status ativo/inativo

        /// <summary>
        /// Construtor protegido para EF Core
        /// </summary>
        protected OrderLine() { }

        /// <summary>
        /// Construtor para criar uma nova linha de pedido
        /// </summary>
        /// <param name="supplierOrderId">ID do pedido do fornecedor</param>
        /// <param name="productId">ID do produto</param>
        /// <param name="quantity">Quantidade (deve ser maior que zero)</param>
        /// <param name="unitPrice">Preço unitário (deve ser maior que zero)</param>
        public OrderLine(Guid supplierOrderId, Guid productId, int quantity, decimal unitPrice)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantidade deve ser maior que zero", nameof(quantity));

            if (unitPrice <= 0)
                throw new ArgumentException("Preço unitário deve ser maior que zero", nameof(unitPrice));

            SupplierOrderId = supplierOrderId;
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
            IsActive = true;
        }

        /// <summary>
        /// Calcula o valor total da linha do pedido
        /// </summary>
        public decimal CalculateTotal()
        {
            return Quantity * UnitPrice;
        }

        /// <summary>
        /// Desativa a linha do pedido
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa a linha do pedido
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}
