using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class TimeSheetEntityConfiguration : IEntityTypeConfiguration<TimeSheet>
    {
        public void Configure(EntityTypeBuilder<TimeSheet> builder)
        {
            builder.ToTable("TimeSheets");

            // Chave primária
            builder.HasKey(t => t.Id);

            // Relacionamento com Employee (N:1)
            builder.HasOne(t => t.Employee)
                   .WithMany(e => e.TimeSheets) // Assumindo que Employee tem ICollection<TimeSheet>
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com TimeSheetOvertime (N:N intermediária)
            builder.HasMany(t => t.TimeSheetOvertimes)
                   .WithOne()
                   .HasForeignKey(ts => ts.TimeSheetId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Propriedades obrigatórias
            builder.Property(t => t.Date)
                   .IsRequired();

            builder.Property(t => t.EntryTime)
                   .IsRequired();

            builder.Property(t => t.ExitTime)
                   .IsRequired();

            builder.Property(t => t.BreakTime)
                   .IsRequired();

            builder.Property(t => t.TotalHours)
                   .IsRequired()
                   .HasColumnType("decimal(5,2)");

            builder.Property(t => t.IsActive)
                   .IsRequired();

            builder.Property(t => t.Observation)
                   .HasMaxLength(500);

            // Enum (JustificationReason) como int
            builder.Property(t => t.JustificationReason)
                   .HasConversion<int?>();
            
        }
    }
}
