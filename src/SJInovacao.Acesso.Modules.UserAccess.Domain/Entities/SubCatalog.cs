using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um Subcatálogo no sistema
    /// Implementa IDeactivatable para permitir ativação/desativação
    /// </summary>
    public class SubCatalog : BaseEntity, IDeactivatable
    {
        // Propriedades básicas do subcatálogo
        public string Name { get; private set; } = string.Empty;         // Nome do subcatálogo
        public string Description { get; private set; } = string.Empty;    // Descrição detalhada
        public bool IsActive { get; private set; } = true;        // Indica se o subcatálogo está ativo

        // Relacionamentos
        public Guid CatalogId { get; private set; }        // ID do catálogo pai
        public Catalog Catalog { get; private set; } = null!;      // Navegação para o catálogo pai

        protected SubCatalog() { }

        //// Lista de produtos associados
        //private readonly List<Product> _products = new();
        //public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        /// <summary>
        /// Construtor para criação de um novo subcatálogo
        /// </summary>
        /// <param name="name">Nome do subcatálogo</param>
        /// <param name="description">Descrição do subcatálogo</param>
        /// <param name="catalog">Catálogo pai ao qual o subcatálogo pertence</param>
        /// <exception cref="ArgumentException">Lançada quando nome ou descrição são vazios</exception>
        /// <exception cref="ArgumentNullException">Lançada quando o catálogo pai é nulo</exception>
        public SubCatalog(string name, string description, Catalog catalog)
        {
            // Validações dos parâmetros
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do subcatálogo não pode ser vazio ou nulo.", nameof(name));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição do subcatálogo não pode ser vazia ou nula.", nameof(description));

            // Atribuição das propriedades
            Name = name;
            Description = description;
            IsActive = true; // Por padrão, o subcatálogo é criado como ativo
            Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog), "O catálogo pai não pode ser nulo.");
            CatalogId = catalog.Id; // Descomentado para garantir a consistência do ID
        }

        ///// <summary>
        ///// Adiciona um produto ao subcatálogo
        ///// </summary>
        ///// <param name="product">Produto a ser adicionado</param>
        ///// <exception cref="ArgumentNullException">Lançada quando o produto é nulo</exception>
        ///// <exception cref="InvalidOperationException">Lançada quando o produto já existe no subcatálogo</exception>
        //public void AddProduct(Product product)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product), "O produto não pode ser nulo.");

        //    if (_products.Contains(product))
        //        throw new InvalidOperationException("O produto já existe no subcatálogo.");

        //    _products.Add(product);
        //}

        ///// <summary>
        ///// Remove um produto do subcatálogo
        ///// </summary>
        ///// <param name="product">Produto a ser removido</param>
        ///// <exception cref="ArgumentNullException">Lançada quando o produto é nulo</exception>
        ///// <exception cref="InvalidOperationException">Lançada quando o produto não é encontrado</exception>
        //public void RemoveProduct(Product product)
        //{
        //    if (product == null)
        //        throw new ArgumentNullException(nameof(product), "O produto não pode ser nulo.");

        //    if (!_products.Remove(product))
        //        throw new InvalidOperationException("Produto não encontrado no subcatálogo.");
        //}

        ///// <summary>
        ///// Verifica se o subcatálogo contém um produto específico
        ///// </summary>
        ///// <param name="product">Produto a ser verificado</param>
        ///// <returns>True se o produto existir no subcatálogo, False caso contrário</returns>
        //public bool ContainsProduct(Product product) => _products.Contains(product);

        /// <summary>
        /// Desativa o subcatálogo
        /// </summary>
        public void Deactivate() => IsActive = false;

        /// <summary>
        /// Ativa o subcatálogo
        /// </summary>
        public void Activate() => IsActive = true;

        /// <summary>
        /// Atualiza o nome do subcatálogo
        /// </summary>
        /// <param name="newName">Novo nome para o subcatálogo</param>
        /// <exception cref="ArgumentException">Lançada quando o novo nome é vazio ou nulo</exception>
        public void UpdateName(string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                throw new ArgumentException("O nome do subcatálogo não pode ser vazio ou nulo.", nameof(newName));

            Name = newName;
        }

        /// <summary>
        /// Atualiza a descrição do subcatálogo
        /// </summary>
        /// <param name="newDescription">Nova descrição para o subcatálogo</param>
        /// <exception cref="ArgumentException">Lançada quando a nova descrição é vazia ou nula</exception>
        public void UpdateDescription(string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("A descrição do subcatálogo não pode ser vazia ou nula.", nameof(newDescription));

            Description = newDescription;
        }
    }
}