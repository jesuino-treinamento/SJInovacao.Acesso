using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class TimeBankEntityConfiguration : IEntityTypeConfiguration<TimeBank>
    {
        public void Configure(EntityTypeBuilder<TimeBank> builder)
        {
            builder.ToTable("TimeBanks");

            // Chave primária
            builder.HasKey(tb => tb.Id);

            // Relacionamento com Employee (1:1)
            builder.HasOne(tb => tb.Employee)
                   .WithOne(e => e.TimeBank) // Assumindo que Employee tem TimeBank
                   .HasForeignKey<TimeBank>(tb => tb.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Propriedades
            builder.Property(tb => tb.EmployeeId)
                   .IsRequired();

            builder.Property(tb => tb.HoursBalance)
                   .IsRequired()
                   .HasColumnType("decimal(10,2)");

            builder.Property(tb => tb.IsActive)
                   .IsRequired();
        }
    }
}
