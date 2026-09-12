using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class CustomerOrderEntityConfiguration : IEntityTypeConfiguration<CustomerOrder>
    {
        public void Configure(EntityTypeBuilder<CustomerOrder> builder)
        {
            builder.ToTable("CustomerOrders");

            builder.HasKey(co => co.Id);

            // Relacionamento obrigatório com Customer (muitos para um)
            builder.HasOne(co => co.Customer)
                .WithMany(c => c.CustomerOrders)
                .HasForeignKey(co => co.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento opcional com Branch (muitos para um)
            builder.HasOne(co => co.Branch)
                .WithMany(b => b.CustomerOrders)
                .HasForeignKey(co => co.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relacionamento 1 para muitos com OrderItems
            builder.HasMany(co => co.OrderItems)
                .WithOne(oi => oi.CustomerOrder)
                .HasForeignKey(oi => oi.CustomerOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(co => co.Date)
                .IsRequired();

            builder.Property(co => co.IsActive)
                .IsRequired();

            // Configura o acesso à coleção privada _orderItems (se você usar)
            builder.Metadata
                .FindNavigation(nameof(CustomerOrder.OrderItems))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
