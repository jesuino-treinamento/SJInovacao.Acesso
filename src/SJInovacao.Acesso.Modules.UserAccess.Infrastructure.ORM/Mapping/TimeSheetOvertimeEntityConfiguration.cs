using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class TimeSheetOvertimeEntityConfiguration : IEntityTypeConfiguration<TimeSheetOvertime>
    {
        public void Configure(EntityTypeBuilder<TimeSheetOvertime> builder)
        {
            builder.ToTable("TimeSheetOvertimes");

            // Definindo chave composta
            builder.HasKey(tso => new { tso.TimeSheetId, tso.OvertimeId });

            // Relacionamento com TimeSheet (N:1)
            builder.HasOne(tso => tso.TimeSheet)
                   .WithMany(ts => ts.TimeSheetOvertimes)
                   .HasForeignKey(tso => tso.TimeSheetId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relacionamento com Overtime (N:1)
            builder.HasOne(tso => tso.Overtime)
                   .WithMany(o => o.TimeSheetOvertimes)
                   .HasForeignKey(tso => tso.OvertimeId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
