using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class AccountPayableEntityConfiguration : IEntityTypeConfiguration<AccountPayable>
    {
        public void Configure(EntityTypeBuilder<AccountPayable> builder)
        {
            builder.ToTable("AccountPayables");

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

            builder.Property(x => x.SupplierId)
                .IsRequired();

            builder.HasOne(x => x.Supplier)
                .WithMany(x => x.AccountsPayables)
                .HasForeignKey(x => x.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
