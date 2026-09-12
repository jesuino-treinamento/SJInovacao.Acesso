//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
//using System.Reflection.Emit;

//namespace SJInovacao.Acesso.Infrastructure.ORM.Mappings
//{
//    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
//    {
//        public void Configure(EntityTypeBuilder<Employee> builder)
//        {
//            //builder.ToTable("Employees");

//            //builder.HasKey(e => e.Id);
//            //builder.Property(e => e.Id)
//            //       .HasColumnType("uuid")
//            //       .HasDefaultValueSql("gen_random_uuid()");

//            // Relacionamento com User
//            builder.HasOne(e => e.User)
//               .WithMany(u => u.Employees)
//               .HasForeignKey(e => e.UserId)
//               .OnDelete(DeleteBehavior.Restrict);

//            builder.HasOne(e => e.Branch)
//                  .WithMany()
//                  .HasForeignKey(e => e.BranchId)
//                  .OnDelete(DeleteBehavior.Restrict);

//            // Mapeamento 1:1 com TimeBank
//            builder.HasOne(e => e.TimeBank)
//                   .WithOne(tb => tb.Employee)
//                   .HasForeignKey<TimeBank>("EmployeeId")
//                   .IsRequired()
//                   .OnDelete(DeleteBehavior.Cascade);

//            builder.Property(e => e.DateAdmission).IsRequired();
//            builder.Property(e => e.DateDismissal);



//            //builder.OwnsOne(p => p.Document, d =>
//            //{
//            //    d.Property(doc => doc.Number)
//            //        .HasColumnName("DocumentoNumero")
//            //        .HasMaxLength(14)
//            //        .IsRequired();

//            //    d.Property(doc => doc.PersonType)
//            //        .HasColumnName("DocumentType")
//            //        .HasConversion<string>()
//            //        .IsRequired();
//            //});



//            // Campos adicionais
//            builder.Property(e => e.RegistrationNumber).HasMaxLength(12).IsRequired();
//            builder.Property(e => e.Photo);
//            builder.Property(e => e.RG).HasMaxLength(20);
//            builder.Property(e => e.BirthDate).IsRequired();
//            builder.Property(e => e.HireDate).IsRequired();
//            builder.Property(e => e.TerminationDate);
//            builder.Property(e => e.MaritalStatus);
//            builder.Property(e => e.EducationLevel);
//            builder.Property(e => e.FatherName).HasMaxLength(100);
//            builder.Property(e => e.MotherName).HasMaxLength(100);
//            builder.Property(e => e.VoterId).HasMaxLength(20);
//            builder.Property(e => e.MilitaryId).HasMaxLength(20);
//            builder.Property(e => e.PIS).HasMaxLength(20);
//            builder.Property(e => e.PASEP).HasMaxLength(20);
//            builder.Property(e => e.WorkCard).HasMaxLength(20);
//        }
//    }
//}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Infrastructure.ORM.Mappings
{
    public class EmployeeEntityConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                   .HasColumnType("uuid")
                   .HasDefaultValueSql("gen_random_uuid()");

            // Relacionamento com User
            builder.HasOne(e => e.User)
                   .WithMany(u => u.Employees)
                   .HasForeignKey(e => e.UserId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Branch)
                   .WithMany(b => b.Employees) // se Branch tiver ICollection<Employee>
                   .HasForeignKey(e => e.BranchId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Mapeamento 1:1 com TimeBank
            builder.HasOne(e => e.TimeBank)
                   .WithOne(tb => tb.Employee)
                   .HasForeignKey<TimeBank>("EmployeeId")
                   .IsRequired()
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.DateAdmission).IsRequired();
            builder.Property(e => e.DateDismissal);

            // Configurar o Value Object Document (owned)
            builder.OwnsOne(e => e.Document, d =>
            {
                d.Property(doc => doc.Number)
                    .HasColumnName("DocumentNumber")
                    .HasMaxLength(18)
                    .IsRequired();

                d.Property(doc => doc.PersonType)
                    .HasColumnName("PersonType")
                    .HasConversion<string>()
                    .IsRequired();
            });

            // Se Employee tiver Name (value object), configurar também
            // builder.OwnsOne(e => e.Name, n => ...);

            // Campos adicionais
            builder.Property(e => e.RegistrationNumber).HasMaxLength(12).IsRequired();
            builder.Property(e => e.Photo);
            builder.Property(e => e.RG).HasMaxLength(20);
            builder.Property(e => e.BirthDate).IsRequired();
            builder.Property(e => e.HireDate).IsRequired();
            builder.Property(e => e.TerminationDate);
            builder.Property(e => e.MaritalStatus);
            builder.Property(e => e.EducationLevel);
            builder.Property(e => e.FatherName).HasMaxLength(100);
            builder.Property(e => e.MotherName).HasMaxLength(100);
            builder.Property(e => e.VoterId).HasMaxLength(20);
            builder.Property(e => e.MilitaryId).HasMaxLength(20);
            builder.Property(e => e.PIS).HasMaxLength(20);
            builder.Property(e => e.PASEP).HasMaxLength(20);
            builder.Property(e => e.WorkCard).HasMaxLength(20);

            // Configurar coleções (se aplicável)
            builder.HasMany(e => e.Activities)
                   .WithOne(a => a.Employee)
                   .HasForeignKey(a => a.EmployeeId);

            builder.HasMany(e => e.Salaries)
                   .WithOne(s => s.Employee)
                   .HasForeignKey(s => s.EmployeeId);

            builder.HasMany(e => e.TimeSheets)
                   .WithOne(ts => ts.Employee)
                   .HasForeignKey(ts => ts.EmployeeId);

            builder.HasMany(e => e.Overtimes)
                   .WithOne(o => o.Employee)
                   .HasForeignKey(o => o.EmployeeId);

            builder.HasMany(e => e.Vacations)
                   .WithOne(v => v.Employee)
                   .HasForeignKey(v => v.EmployeeId);

            builder.HasMany(e => e.TimeSheetOvertimes)
                   .WithOne(tso => tso.Employee)
                   .HasForeignKey(tso => tso.EmployeeId);

            builder.HasMany(e => e.EmployeePayments)
                   .WithOne(ep => ep.Employee)
                   .HasForeignKey(ep => ep.EmployeeId);
        }
    }
}
