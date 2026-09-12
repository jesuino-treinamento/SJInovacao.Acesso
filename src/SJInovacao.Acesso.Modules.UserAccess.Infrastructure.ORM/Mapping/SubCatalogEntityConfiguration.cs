using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class SubCatalogEntityConfiguration : IEntityTypeConfiguration<SubCatalog>
    {
        public void Configure(EntityTypeBuilder<SubCatalog> builder)
        {
            builder.ToTable("SubCatalogs");

            // Chave primária
            builder.HasKey(sc => sc.Id);

            // Propriedades simples
            builder.Property(sc => sc.Name)
                .IsRequired()
                .HasMaxLength(100); // Defina o tamanho máximo conforme necessário

            builder.Property(sc => sc.Description)
                .IsRequired()
                .HasMaxLength(500); // Defina o tamanho máximo conforme necessário

            builder.Property(sc => sc.IsActive)
                .IsRequired();

            builder.Property(sc => sc.CatalogId)
                .IsRequired();

            // Relacionamento com Catalog (N:1)
            builder.HasOne(sc => sc.Catalog)
                .WithMany(c => c.SubCatalogs) // Assumindo que Catalog tem ICollection<SubCatalog> SubCatalogs
                .HasForeignKey(sc => sc.CatalogId)
                .OnDelete(DeleteBehavior.Restrict);

            //// Relacionamento com Product (1:N) via _products
            //builder.HasMany<Product>("_products")
            //    .WithOne(p => p.SubCatalog) // Assumindo que Product tem uma propriedade SubCatalog
            //    .HasForeignKey(p => p.SubCatalogId) // Assumindo que Product tem a FK SubCatalogId
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Ignorar a propriedade de leitura Products (que expõe _products como IReadOnlyCollection)
            //builder.Metadata.FindNavigation(nameof(SubCatalog.Products))!
            //    .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
