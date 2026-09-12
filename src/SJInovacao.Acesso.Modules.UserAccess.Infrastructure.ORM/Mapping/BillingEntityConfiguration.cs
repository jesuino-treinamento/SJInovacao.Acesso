using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class BillingEntityConfiguration : IEntityTypeConfiguration<Billing>
    {
        public void Configure(EntityTypeBuilder<Billing> builder)
        {
            builder.ToTable("Billings");

            builder.HasKey(x => x.Id);

            builder.HasOne(b => b.Customer)
               .WithMany(c => c.Billings)
               .HasForeignKey(b => b.CustomerId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.AccountReceivables)
                .WithOne(ar => ar.Billing)
                .HasForeignKey(ar => ar.BillingId);

            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.IssueDate)
                .IsRequired();

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.IsPaid)
                .IsRequired();

            //builder.Property(x => x.CustomerId)
            //    .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            
        }
    }
}
