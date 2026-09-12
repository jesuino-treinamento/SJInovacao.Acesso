using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class GroupPermissionEntityConfiguration : IEntityTypeConfiguration<GroupPermission>
    {
        //public void Configure(EntityTypeBuilder<GroupPermission> builder)
        //{

        //    builder.ToTable("GroupAccess");

        //    builder.HasKey(g => g.Id);
        //    builder.Property(g => g.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        //    builder.Property(g => g.Name)
        //        .IsRequired()
        //        .HasMaxLength(100);

        //    builder.Property(g => g.Description)
        //        .IsRequired()
        //        .HasMaxLength(500);

        //    builder.HasMany(g => g.Users)
        //           .WithMany(p => p.Groups)
        //           .UsingEntity(j => j.ToTable("UserGroups"));
        //}
        public void Configure(EntityTypeBuilder<GroupPermission> builder)
        {
            builder.ToTable("GroupAccess");

            builder.HasKey(g => g.Id);
            builder.Property(g => g.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            builder.Property(g => g.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(g => g.Description)
                .IsRequired()
                .HasMaxLength(250);

            builder.Property(u => u.IsActive).IsRequired();

            builder.HasMany(g => g.UserGroups)
               .WithOne(ug => ug.Group)
               .HasForeignKey(ug => ug.GroupId);
        }

    }
}
