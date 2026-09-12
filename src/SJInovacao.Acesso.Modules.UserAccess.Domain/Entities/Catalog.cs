using SJInovacao.Acesso.Modules.UserAccess.Domain.Common;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Specifications;

namespace SJInovacao.Acesso.Modules.UserAccess.Domain.Entities
{
    /// <summary>
    /// Classe que representa um Catálogo no sistema.
    /// Implementa IDeactivatable para permitir ativação/desativação.
    /// </summary>
    public class Catalog : BaseEntity, IDeactivatable
    {
        // Propriedades básicas do catálogo
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public bool IsActive { get; private set; } = true;

        // Relacionamento com produtos associados a este catálogo
        public ICollection<Product> Products { get; private set; } = new List<Product>();

        // Relacionamento com subcatálogos
        private readonly List<SubCatalog> _subCatalogs = new();
        public IReadOnlyCollection<SubCatalog> SubCatalogs => _subCatalogs.AsReadOnly();

        // Construtor

        public Catalog()
        {
                
        }
        public Catalog(string name, string description)
        {
            SetName(name);
            SetDescription(description);
        }

        // Atualização do nome
        public void UpdateName(string newName) => SetName(newName);

        // Atualização da descrição
        public void UpdateDescription(string newDescription) => SetDescription(newDescription);

        // Ativação
        public void Activate()
        {
            if (!IsActive)
                IsActive = true;
        }

        // Desativação
        public void Deactivate()
        {
            if (IsActive)
                IsActive = false;
        }

        // Adição de subcatálogo
        public void AddSubCatalog(SubCatalog subCatalog)
        {
            if (subCatalog is null)
                throw new ArgumentNullException(nameof(subCatalog));

            _subCatalogs.Add(subCatalog);
        }

        // Remoção de subcatálogo
        public void RemoveSubCatalog(SubCatalog subCatalog)
        {
            if (subCatalog is null)
                throw new ArgumentNullException(nameof(subCatalog));

            if (!_subCatalogs.Remove(subCatalog))
                throw new InvalidOperationException("Subcatálogo não encontrado no catálogo.");
        }

        // Verificação
        public bool ContainsSubCatalog(SubCatalog subCatalog)
        {
            return _subCatalogs.Contains(subCatalog);
        }

        // Validações internas
        private void SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do catálogo não pode ser vazio ou nulo.", nameof(name));

            Name = name;
        }

        private void SetDescription(string description)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("A descrição do catálogo não pode ser vazia ou nula.", nameof(description));

            Description = description;
        }
    }
}
