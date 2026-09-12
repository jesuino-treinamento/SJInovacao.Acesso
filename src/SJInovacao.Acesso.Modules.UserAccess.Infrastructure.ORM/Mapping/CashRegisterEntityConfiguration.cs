using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class CashRegisterEntityConfiguration : IEntityTypeConfiguration<CashRegister>
    {
        public void Configure(EntityTypeBuilder<CashRegister> builder)
        {
            builder.ToTable("CashRegisters");

            builder.HasKey(cr => cr.Id);            

            // Relacionamento opcional com User (1:N)
            builder.HasOne(cr => cr.User)
                .WithMany()
                .HasForeignKey(cr => cr.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relacionamento opcional com Branch (1:N)
            builder.HasOne(cr => cr.Branch)
                .WithMany(b => b.CashRegisters)
                .HasForeignKey(cr => cr.BranchId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.Property(cr => cr.CurrentBalance)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(cr => cr.IsActive)
                .IsRequired();
           

            // Mapear coleção privada _movements para CashMovements
            builder.Metadata
                .FindNavigation(nameof(CashRegister.CashMovements))!
                .SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasMany(cr => cr.CashMovements)
                .WithOne(cm => cm.CashRegister)
                .HasForeignKey(cm => cm.CashRegisterId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
