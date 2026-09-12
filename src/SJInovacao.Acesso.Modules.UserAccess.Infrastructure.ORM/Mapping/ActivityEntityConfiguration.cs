using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class ActivityEntityConfiguration : IEntityTypeConfiguration<Activity>
    {
        public void Configure(EntityTypeBuilder<Activity> builder)
        {
            builder.ToTable("Activities");

            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");


            builder.Property(a => a.Position)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Function)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Workplace)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(a => a.Workload)
                .IsRequired();

            builder.Property(a => a.EmployeeId)
                .IsRequired();

            builder.Property(a => a.IsActive)
                .IsRequired();

            builder.HasOne(a => a.Employee)
                .WithMany(a => a.Activities)
                .HasForeignKey(a => a.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
