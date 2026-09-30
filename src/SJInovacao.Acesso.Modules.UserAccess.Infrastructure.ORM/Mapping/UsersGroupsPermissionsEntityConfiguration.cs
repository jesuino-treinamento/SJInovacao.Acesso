using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UsersGroupsPermissionsEntityConfiguration : IEntityTypeConfiguration<UsersGroupsPermissions>
    {
        public void Configure(EntityTypeBuilder<UsersGroupsPermissions> builder)
        {
            builder.ToTable("UsersGroupsPermissions");

            builder.HasKey(ugp => new { ugp.UserId, ugp.GroupId, ugp.PermissionId });

            builder.HasOne(ugp => ugp.User)
                   .WithMany(u => u.UsersGroupsPermissions)
                   .HasForeignKey(ugp => ugp.UserId);

            builder.HasOne(ugp => ugp.Group)
                   .WithMany(g => g.UsersGroupsPermissions)
                   .HasForeignKey(ugp => ugp.GroupId);

            builder.HasOne(ugp => ugp.Permission)
                   .WithMany(p => p.UsersGroupsPermissions)
                   .HasForeignKey(ugp => ugp.PermissionId);

            builder.Property(ugp => ugp.IsActive).HasDefaultValue(true);
            builder.Property(ugp => ugp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(ugp => ugp.UpdatedAt);
        }
    }

}
