using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class CatalogEntityConfiguration : IEntityTypeConfiguration<Catalog>
    {
        public void Configure(EntityTypeBuilder<Catalog> builder)
        {
            builder.ToTable("Catalogs");

            builder.HasKey(c => c.Id);

            // Relacionamento Catalog 1:N Product
            builder.HasMany(c => c.Products)
                   .WithOne(p => p.Catalog)
                   .HasForeignKey(p => p.CatalogId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(c => c.IsActive)
                .IsRequired();
        }
    }
}
