using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class SupplierOrderEntityConfiguration : IEntityTypeConfiguration<SupplierOrder>
    {
        public void Configure(EntityTypeBuilder<SupplierOrder> builder)
        {
            builder.ToTable("SupplierOrders");

            // Chave primária
            builder.HasKey(so => so.Id);

            // Relacionamento com Supplier (N:1)
            builder.HasOne(so => so.Supplier)
                .WithMany(s => s.SupplierOrders) // Assumindo ICollection<SupplierOrder> SupplierOrders na entidade Supplier
                .HasForeignKey(so => so.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com OrderLines (1:N)
            builder.HasMany(so => so.Lines)
                .WithOne(ol => ol.SupplierOrder) // Assumindo que OrderLine tem propriedade SupplierOrder
                .HasForeignKey(ol => ol.SupplierOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Propriedades simples
            builder.Property(so => so.Date)
                .IsRequired();

            builder.Property(so => so.IsActive)
                .IsRequired();

            builder.Property(so => so.SupplierId)
                .IsRequired();
        }
    }
}
