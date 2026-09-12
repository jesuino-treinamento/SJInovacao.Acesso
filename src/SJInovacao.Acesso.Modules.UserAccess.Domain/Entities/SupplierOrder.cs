using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um pedido de compra para fornecedor
    /// Implementa IDesativavel para permitir desativação/ativação
    /// </summary>
    public class SupplierOrder : BaseEntity, IDeactivatable
    {
        public DateTime Date { get; private set; }                     // Data do pedido
        public Guid SupplierId { get; private set; }                  // ID do fornecedor
        public Supplier Supplier { get; private set; } = null!;      // Navegação para o fornecedor
        public ICollection<OrderLine> Lines { get; private set; } = new List<OrderLine>();    // Itens do pedido fornecedor
        public bool IsActive { get; private set; }                     // Status ativo/inativo

        /// <summary>
        /// Construtor protegido para EF Core
        /// </summary>
        protected SupplierOrder() { }

        /// <summary>
        /// Construtor para criar um novo pedido
        /// </summary>
        /// <param name="date">Data do pedido</param>
        public SupplierOrder(DateTime date)
        {
            IsActive = true;
            Date = date;
            Lines = new List<OrderLine>();
        }

        /// <summary>
        /// Adiciona uma linha de item ao pedido
        /// </summary>
        /// <param name="line">Linha do pedido</param>
        /// <exception cref="ArgumentException">Quando quantidade for menor ou igual a zero</exception>
        public void AddLine(OrderLine line)
        {
            if (line?.Quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            Lines.Add(line ?? throw new ArgumentNullException(nameof(line)));
        }

        /// <summary>
        /// Remove uma linha de item do pedido
        /// </summary>
        /// <param name="line">Linha do pedido</param>
        /// <exception cref="ArgumentException">Quando quantidade for menor ou igual a zero</exception>
        public void RemoveLine(OrderLine line)
        {
            if (line?.Quantity <= 0)
                throw new ArgumentException("A quantidade deve ser maior que zero.");

            Lines.Remove(line ?? throw new ArgumentNullException(nameof(line)));
        }

        /// <summary>
        /// Desativa o pedido (marca como inativo)
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o pedido (marca como ativo)
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}