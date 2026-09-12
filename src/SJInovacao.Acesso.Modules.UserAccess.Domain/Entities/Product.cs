using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Enums;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;
using SJInovacao.Acesso.Modules.UserAccess.Domain.ValueObjects;
using System.Net.Mail;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um Produto no sistema
    /// Implementa IDeactivatable para permitir ativação/desativação
    /// </summary>
    public class Product : BaseEntity, IDeactivatable
    {
        // Propriedades básicas do produto
        public string Name { get; private set; } = string.Empty;        // Nome do produto
        public long EAN { get; set; } = 0;                    // Código de barras EAN
        public string SKU { get; set; } = string.Empty;                    // Código SKU único
        public string Description { get; private set; } = string.Empty;    // Descrição detalhada
        public decimal Price { get; private set; } = 0;       // Preço unitário
        public int Size { get; set; } = 0;                    // Tamanho (pode ser numérico ou corresponder a tamanhos como P, M, G)
        public int Weight { get; set; } = 0;                   // Peso em gramas
        public string Color { get; set; } = string.Empty;                 // Cor principal
        public string Measurement { get; set; } = string.Empty;           // Medidas (ex: "10x15x20cm")
        public int StockQuantity { get; private set; } = 0;    // Quantidade em estoque
        public byte[] Photo { get; set; } = Array.Empty<byte>(); // Foto do produto em bytes

        // Relacionamentos
        public Guid? SupplierId { get; set; }            // ID do fornecedor
        public Supplier? Supplier { get; set; }           // Navegação para o fornecedor
        public Guid? CustomerId { get; set; }            // ID do cliente (se aplicável)
        public Customer? Customer { get; set; }            // Navegação para o cliente
        public Guid SubCatalogId { get; private set; }   // ID do subcatálogo
        public SubCatalog? SubCatalog { get; private set; }// Navegação para o subcatálogo
        public Guid CatalogId { get; private set; }       // ID do catálogo principal
        public Catalog? Catalog { get; private set; }      // Navegação para o catálogo principal

        public Guid BranchId { get; private set; }       // ID do branch principal
        public Branch? Branch { get; private set; }      // Navegação para o branch principal

        // Status do produto
        public bool IsActive { get; set; }               // Indica se o produto está ativo

        // Anexos do produto (fotos adicionais, manuais, etc.)
        private readonly List<Attachment> _attachments = new();
        public IReadOnlyCollection<Attachment> Attachments => _attachments.AsReadOnly();

        private readonly List<Catalog> _catalogs = new();
        public IReadOnlyCollection<Catalog> Catalogs => _catalogs.AsReadOnly();

        private readonly List<Inventory> _inventories = new();
        public IReadOnlyCollection<Inventory> Inventories => _inventories.AsReadOnly();

        /// <summary>
        /// Construtor protegido para ORM
        /// </summary>
        protected Product() { }

        /// <summary>
        /// Construtor principal para criação de um novo produto
        /// </summary>
        /// <param name="name">Nome do produto</param>
        /// <param name="description">Descrição detalhada</param>
        /// <param name="price">Preço unitário</param>
        /// <param name="stockQuantity">Quantidade inicial em estoque</param>
        /// <param name="subCatalog">Subcatálogo ao qual o produto pertence</param>
        /// <exception cref="ArgumentException">Lançada quando parâmetros inválidos são fornecidos</exception>
        /// <exception cref="ArgumentNullException">Lançada quando subCatalog é nulo</exception>
        public Product(string name, string description, decimal price, int stockQuantity, SubCatalog subCatalog)
        {
            // Validações dos parâmetros
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do produto não pode ser vazio.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição do produto não pode ser vazia.", nameof(description));

            if (price < 0)
                throw new ArgumentException("O preço do produto não pode ser negativo.", nameof(price));

            if (stockQuantity < 0)
                throw new ArgumentException("A quantidade em estoque não pode ser negativa.", nameof(stockQuantity));

            // Atribuição das propriedades
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            IsActive = true;

            // Configuração do catálogo e subcatálogo
            SubCatalog = subCatalog ?? throw new ArgumentNullException(nameof(subCatalog));
            SubCatalogId = subCatalog.Id;
            Catalog = subCatalog.Catalog ?? throw new ArgumentNullException(nameof(subCatalog.Catalog));
            CatalogId = subCatalog.Catalog.Id;
        }

        /// <summary>
        /// Atualiza a quantidade em estoque do produto
        /// </summary>
        /// <param name="quantity">Quantidade a ser adicionada (positiva) ou removida (negativa)</param>
        /// <exception cref="InvalidOperationException">Lançada quando o estoque ficaria negativo</exception>
        public void UpdateStock(int quantity)
        {
            if (StockQuantity + quantity < 0)
                throw new InvalidOperationException("O estoque não pode ficar negativo.");

            StockQuantity += quantity;

            var movement = new Inventory(Id, quantity,
                quantity > 0 ? MovementType.Inbound : MovementType.Outbound);

            _inventories.Add(movement); // agora armazena o movimento
        }


        /// <summary>
        /// Altera o preço do produto
        /// </summary>
        /// <param name="newPrice">Novo preço do produto</param>
        /// <exception cref="ArgumentException">Lançada quando o novo preço é negativo</exception>
        public void ChangePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("O preço não pode ser negativo.", nameof(newPrice));

            Price = newPrice;
        }

        /// <summary>
        /// Verifica se o estoque está abaixo do mínimo e desativa o produto se necessário
        /// </summary>
        /// <param name="minimum">Quantidade mínima em estoque</param>
        public void CheckMinimumStock(int minimum)
        {
            if (StockQuantity < minimum)
            {
                Deactivate();
            }
        }

        /// <summary>
        /// Atualiza a foto principal do produto
        /// </summary>
        /// <param name="newPhoto">Novo array de bytes contendo a imagem</param>
        /// <exception cref="ArgumentNullException">Lançada quando a nova foto é nula</exception>
        public void UpdatePhoto(byte[] newPhoto)
        {
            Photo = newPhoto ?? throw new ArgumentNullException(nameof(newPhoto));
        }

        /// <summary>
        /// Adiciona um anexo ao produto
        /// </summary>
        /// <param name="attachment">Anexo a ser adicionado</param>
        /// <exception cref="ArgumentNullException">Lançada quando o anexo é nulo</exception>
        /// <exception cref="ArgumentException">Lançada quando o conteúdo do anexo está vazio</exception>
        public void AddAttachment(Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "O anexo não pode ser nulo.");

            if (attachment.Content.Length == 0)
                throw new ArgumentException("O conteúdo do anexo não pode estar vazio.", nameof(attachment));

            _attachments.Add(attachment);
        }

        /// <summary>
        /// Remove um anexo do produto
        /// </summary>
        /// <param name="attachment">Anexo a ser removido</param>
        /// <exception cref="ArgumentNullException">Lançada quando o anexo é nulo</exception>
        public void RemoveAttachment(Attachment attachment)
        {
            if (attachment == null)
                throw new ArgumentNullException(nameof(attachment), "O anexo não pode ser nulo.");

            _attachments.Remove(attachment);
        }

        /// <summary>
        /// Remove todos os anexos do produto
        /// </summary>
        public void ClearAttachments()
        {
            _attachments.Clear();
        }

        /// <summary>
        /// Desativa o produto
        /// </summary>
        public void Deactivate()
        {
            if (!IsActive) return;
            IsActive = false;
        }

        /// <summary>
        /// Ativa o produto
        /// </summary>
        public void Activate()
        {
            if (IsActive) return;
            IsActive = true;
        }
    }
}