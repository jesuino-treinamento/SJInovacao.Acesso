using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Infrastructure.ORM.Mappings
{
    //public class PhoneEntityConfiguration : IEntityTypeConfiguration<Phone>
    //{
    //    public void Configure(EntityTypeBuilder<Phone> builder)
    //    {

    //        builder.HasOne(a => a.Person)
    //            .WithMany(u => u.Phones)
    //            .HasForeignKey(a => a.PersonId)
    //            .OnDelete(DeleteBehavior.Cascade);

    //        builder.HasKey(u => u.Id);
    //        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

    //        builder.Property(t => t.Number)
    //            .IsRequired()
    //            .HasMaxLength(20)
    //            .HasColumnName("Number");

    //        builder.Property(t => t.Type)
    //            .IsRequired()
    //            .HasConversion<string>()
    //            .HasColumnName("Type");

    //        builder.Property(t => t.IsActive)
    //            .IsRequired()
    //            .HasDefaultValue(true);
    //    }
    //}

    public class PhoneEntityConfiguration : IEntityTypeConfiguration<Phone>
    {
        public void Configure(EntityTypeBuilder<Phone> builder)
        {
            builder.ToTable("Phones");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Number).HasMaxLength(20).IsRequired();
            builder.Property(p => p.Type).HasConversion<string>();
            //builder.Property(p => p.IsActive).IsRequired();

            builder.HasOne(p => p.Person)
                .WithMany(p => p.Phones)
                .HasForeignKey(p => p.PersonId);
        }
    }
}
