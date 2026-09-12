using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class CashMovementEntityConfiguration : IEntityTypeConfiguration<CashMovement>
    {
        public void Configure(EntityTypeBuilder<CashMovement> builder)
        {
            builder.ToTable("CashMovements");

            builder.HasKey(cm => cm.Id);

            // Relação com CashRegister (1:N)
            builder.HasOne(cm => cm.CashRegister)
                .WithMany(cr => cr.CashMovements)
                .HasForeignKey(cm => cm.CashRegisterId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relação com User (1:N)
            builder.HasOne(cm => cm.User)
                .WithMany() // supondo que User não tenha coleção de CashMovements
                .HasForeignKey(cm => cm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(cm => cm.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(cm => cm.MovementType)
                .IsRequired();

            builder.Property(cm => cm.MovementDate)
                .IsRequired();

            builder.Property(cm => cm.IsActive)
                .IsRequired();            
        }
    }
}
