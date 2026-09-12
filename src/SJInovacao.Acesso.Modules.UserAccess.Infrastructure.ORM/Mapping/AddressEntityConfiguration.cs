using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Infrastructure.ORM.Mappings
{
    //public class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    //{
    //    public void Configure(EntityTypeBuilder<Address> builder)
    //    {
    //        builder.ToTable("Addresses");

    //        builder.HasKey(u => u.Id);
    //        builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

    //        builder.HasOne(a => a.Person)
    //            .WithMany(u => u.Addresses)
    //            .HasForeignKey(a => a.PersonId)
    //            .OnDelete(DeleteBehavior.Cascade);

    //        builder.Property(a => a.City)
    //            .IsRequired()
    //            .HasMaxLength(100);

    //        builder.Property(a => a.Street)
    //            .IsRequired()
    //            .HasMaxLength(100);

    //        builder.Property(a => a.Number)
    //            .IsRequired();

    //        builder.Property(a => a.ZipCode)
    //            .HasMaxLength(20);

    //        builder.OwnsOne(a => a.Geolocation, location =>
    //        {
    //            location.Property(l => l.Lat)
    //                .HasMaxLength(20)
    //                .HasColumnName("Latitude");

    //            location.Property(l => l.Long)
    //                .HasMaxLength(20)
    //                .HasColumnName("Longitude");
    //        });            
    //    }
    //}

    public class AddressEntityConfiguration : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses");
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Street).HasMaxLength(200).IsRequired();
            builder.Property(a => a.Number).HasMaxLength(20).IsRequired();
            builder.Property(a => a.Neighborhood).HasMaxLength(100).IsRequired();
            builder.Property(a => a.City).HasMaxLength(100).IsRequired();
            builder.Property(a => a.State).HasMaxLength(2).IsRequired();
            builder.Property(a => a.ZipCode).HasMaxLength(9).IsRequired();
            builder.Property(a => a.IsActive).IsRequired();

            builder.OwnsOne(a => a.Geolocation, geo =>
            {
                geo.Property(g => g.Lat).HasColumnName("Latitude").HasMaxLength(20);
                geo.Property(g => g.Long).HasColumnName("Longitude").HasMaxLength(20);
            });

            builder.HasOne(a => a.Person)
                .WithMany(p => p.Addresses)
                .HasForeignKey(a => a.PersonId);
        }
    }
}
