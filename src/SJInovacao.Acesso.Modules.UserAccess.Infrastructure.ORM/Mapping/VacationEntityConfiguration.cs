using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class VacationEntityConfiguration : IEntityTypeConfiguration<Vacation>
    {
        public void Configure(EntityTypeBuilder<Vacation> builder)
        {
            builder.ToTable("Vacations");

            // Chave primária
            builder.HasKey(v => v.Id);

            // Relacionamento com Employee (N:1)
            builder.HasOne(v => v.Employee)
                   .WithMany(e => e.Vacations) // Assumindo ICollection<Vacation> em Employee
                   .HasForeignKey(v => v.EmployeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Propriedades obrigatórias
            builder.Property(v => v.StartDate)
                   .IsRequired();

            builder.Property(v => v.EndDate)
                   .IsRequired();

            builder.Property(v => v.Approved)
                   .IsRequired();

            builder.Property(v => v.IsActive)
                   .IsRequired();
        }
    }
}
