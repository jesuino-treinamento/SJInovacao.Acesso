using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class OrderItemEntityConfiguration : IEntityTypeConfiguration<OrderItem>
    {
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("OrderItems");

            // Chave primária
            builder.HasKey(oi => oi.Id);
            builder.Property(oi => oi.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            // Relacionamento com CustomerOrder (N:1)
            builder.HasOne(oi => oi.CustomerOrder)
                .WithMany(co => co.OrderItems) // pressupõe que CustomerOrder tem ICollection<OrderItem> OrderItems
                .HasForeignKey(oi => oi.CustomerOrderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Product (N:1)
            builder.HasOne(oi => oi.Product)
                .WithMany() // se Product não tem coleção de OrderItems
                .HasForeignKey(oi => oi.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            // Propriedades
            builder.Property(oi => oi.CustomerOrderId)
                .IsRequired();

            builder.Property(oi => oi.ProductId)
                .IsRequired();

            builder.Property(oi => oi.ProductName)
                .IsRequired();

            builder.Property(oi => oi.Quantity)
                .IsRequired();

            builder.Property(oi => oi.UnitPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(oi => oi.TotalPrice)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(oi => oi.Discount)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(oi => oi.CancellationDate)
                .IsRequired(false);

            builder.Property(oi => oi.IsCancelled)
                .IsRequired();

           
        }
    }
}
