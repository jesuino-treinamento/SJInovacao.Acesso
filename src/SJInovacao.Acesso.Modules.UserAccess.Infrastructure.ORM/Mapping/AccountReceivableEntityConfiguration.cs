using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class AccountReceivableEntityConfiguration : IEntityTypeConfiguration<AccountReceivable>
    {
        public void Configure(EntityTypeBuilder<AccountReceivable> builder)
        {
            builder.ToTable("AccountReceivables");

            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");


            builder.Property(x => x.Amount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.DueDate)
                .IsRequired();

            builder.Property(x => x.PaymentDate)
                .IsRequired(false);

            builder.Property(x => x.IsPaid)
                .IsRequired();

            builder.Property(x => x.IsActive)
                .IsRequired();

            builder.Property(x => x.BillingId)
                .IsRequired();

            builder.HasOne(x => x.Billing)
                .WithMany(x => x.AccountReceivables)
                .HasForeignKey(x => x.BillingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
