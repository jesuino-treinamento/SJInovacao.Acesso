using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class GroupsPermissionsEntityConfiguration : IEntityTypeConfiguration<GroupsPermissions>
    {
        public void Configure(EntityTypeBuilder<GroupsPermissions> builder)
        {
            builder.ToTable("GroupsPermissions");

            builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

            builder.HasOne(gp => gp.Group)
                   .WithMany(g => g.GroupsPermissions)
                   .HasForeignKey(gp => gp.GroupId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(gp => gp.Permission)
                   .WithMany(p => p.GroupsPermissions)
                   .HasForeignKey(gp => gp.PermissionId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(gp => gp.IsActive).IsRequired();
            builder.Property(gp => gp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(gp => gp.UpdatedAt);
        }
    }

}
