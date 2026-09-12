using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    //public class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
    //{
    //    public void Configure(EntityTypeBuilder<Person> builder)
    //    {
    //        builder.ToTable("Persons");

    //        builder.HasKey(u => u.Id);
    //        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

    //        builder.OwnsOne(p => p.Name, name =>
    //        {
    //            name.Property(n => n.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(100);
    //            name.Property(n => n.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(100);
    //        });

    //        builder.OwnsOne(p => p.Document, d =>
    //        {
    //            d.Property(doc => doc.Number)
    //                .HasColumnName("DocumentoNumero")
    //                .HasMaxLength(14)
    //                .IsRequired();

    //            d.Property(doc => doc.PersonType)
    //                .HasColumnName("PersonType")
    //                .IsRequired()
    //                .HasConversion<String>();
    //        });

    //        builder.HasMany(p => p.Phones)
    //               .WithOne(p => p.Person)           // Assumindo que Phone tem navegação para Person
    //               .HasForeignKey(p => p.PersonId)
    //               .IsRequired();

    //        builder.HasMany(p => p.Addresses)
    //               .WithOne(a => a.Person)          // Assumindo que Address tem navegação para Person
    //               .HasForeignKey(a => a.PersonId)
    //               .IsRequired();

    //        //// Configura o discriminador para diferenciar Person, User, etc.
    //        //builder.HasDiscriminator<string>("PersonDiscriminator")
    //        //    .HasValue<Person>("Person")
    //        //    .HasValue<User>("User")
    //        //    .HasValue<Employee>("Employee")
    //        //    .HasValue<Customer>("Customer")
    //        //    .HasValue<Supplier>("Supplier");
    //    }
    //}

    public class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");
            builder.HasKey(p => p.Id);
            builder.OwnsOne(p => p.Name, n =>
            {
                n.Property(nn => nn.FirstName).HasColumnName("FirstName").HasMaxLength(100);
                n.Property(nn => nn.LastName).HasColumnName("LastName").HasMaxLength(100);
            });
            builder.OwnsOne(p => p.Document, d =>
            {
                d.Property(doc => doc.Number).HasColumnName("DocumentNumber").HasMaxLength(18);
                d.Property(doc => doc.PersonType).HasColumnName("PersonType").HasConversion<string>(); 
            });
           
        }
    }
}
