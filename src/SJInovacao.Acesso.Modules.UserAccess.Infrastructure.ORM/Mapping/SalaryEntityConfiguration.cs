using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class SalaryEntityConfiguration : IEntityTypeConfiguration<Salary>
    {
        public void Configure(EntityTypeBuilder<Salary> builder)
        {
            builder.ToTable("Salaries");

            // Chave primária
            builder.HasKey(s => s.Id);

            // Relacionamento com Employee (N:1)
            builder.HasOne(s => s.Employee)
                .WithMany(e => e.Salaries)
                .HasForeignKey(s => s.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com EmployeePayments (1:N)
            builder.HasMany(s => s.EmployeePayments)
                .WithOne(ep => ep.Salary)
                .HasForeignKey(ep => ep.SalaryId)
                .OnDelete(DeleteBehavior.Cascade);

            // Propriedades obrigatórias e tipos de dados
            builder.Property(s => s.BaseSalary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.CurrentSalary)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(s => s.EmployeeId)
                .IsRequired();

            builder.Property(s => s.IsActive)
                .IsRequired();
        }
    }
}
