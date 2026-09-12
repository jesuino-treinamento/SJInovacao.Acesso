using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class OrderLineEntityConfiguration : IEntityTypeConfiguration<OrderLine>
    {
        public void Configure(EntityTypeBuilder<OrderLine> builder)
        {
            builder.ToTable("OrderLines");

            // Chave primária
            builder.HasKey(ol => ol.Id);

            // Relacionamento com Product (1:N)
            builder.HasOne(ol => ol.Product)
                .WithMany()
                .HasForeignKey(ol => ol.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com SupplierOrder (1:N reverso ou N:1 dependendo do modelo)
            builder.HasOne(ol => ol.SupplierOrder)
                .WithMany() // Ou .WithMany(so => so.OrderLines) se existir uma coleção do outro lado
                .HasForeignKey(ol => ol.SupplierOrderId) // Propriedade sombra se não existir explicitamente
                .IsRequired(false) // Pode ser opcional, depende da lógica
                .OnDelete(DeleteBehavior.Restrict);

            // Propriedades obrigatórias
            builder.Property(ol => ol.ProductId)
                .IsRequired();

            builder.Property(ol => ol.Quantity)
                .IsRequired();

            builder.Property(ol => ol.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ol => ol.IsActive)
                .IsRequired();
        }
    }
}
