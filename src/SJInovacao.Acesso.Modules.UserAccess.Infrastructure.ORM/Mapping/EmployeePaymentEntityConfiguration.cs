using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class EmployeePaymentEntityConfiguration : IEntityTypeConfiguration<EmployeePayment>
    {
        public void Configure(EntityTypeBuilder<EmployeePayment> builder)
        {
            builder.ToTable("EmployeePayments");

            builder.HasKey(ep => ep.Id);

            // Relacionamento obrigatório com Employee
            builder.HasOne(ep => ep.Employee)
                .WithMany(e => e.EmployeePayments)
                .HasForeignKey(ep => ep.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento obrigatório com Salary
            builder.HasOne(ep => ep.Salary)
                .WithMany(s => s.EmployeePayments)
                .HasForeignKey(ep => ep.SalaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(ep => ep.AmountPaid)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(ep => ep.PaymentDate)
                .IsRequired();

            builder.Property(ep => ep.Overtime50)
                .HasColumnType("decimal(10,2)");

            builder.Property(ep => ep.Overtime70)
                .HasColumnType("decimal(10,2)");

            builder.Property(ep => ep.Overtime100)
                .HasColumnType("decimal(10,2)");

            builder.Property(ep => ep.IsActive)
                .IsRequired();
        }
    }
}
