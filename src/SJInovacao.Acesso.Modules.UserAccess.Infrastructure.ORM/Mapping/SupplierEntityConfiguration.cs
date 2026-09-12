using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Infrastructure.ORM.Mappings
{
    public class SupplierEntityConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            //builder.HasKey(u => u.Id);
            //builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            //builder.Property(s => s.CreatedAt).IsRequired();
            //builder.Property(s => s.UpdatedAt);

            builder.HasOne(e => e.User)
                  .WithMany(u => u.Suppliers)
                  .HasForeignKey("UserId")
                  .OnDelete(DeleteBehavior.Restrict);

            // Não usar ToTable nem HasKey nem Id - herdado de User/Person

            builder.Property(s => s.StateRegistration)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.MunicipalRegistration)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(s => s.TradeName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(s => s.RegistrationDate)
                .IsRequired();

            builder.HasMany(s => s.SupplierOrders)
                .WithOne(so => so.Supplier)
                .HasForeignKey(so => so.SupplierId);

            builder.HasMany(s => s.AccountsPayables)
                .WithOne(ap => ap.Supplier)
                .HasForeignKey(ap => ap.SupplierId);

            builder.HasMany(s => s.Products)
                .WithOne(p => p.Supplier)
                .HasForeignKey(p => p.SupplierId);
        }
    }
}
