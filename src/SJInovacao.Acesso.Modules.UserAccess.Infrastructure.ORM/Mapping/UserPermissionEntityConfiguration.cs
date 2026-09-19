using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UserPermissionEntityConfiguration : IEntityTypeConfiguration<UserPermission>
    {
        public void Configure(EntityTypeBuilder<UserPermission> builder)
        {
            builder.ToTable("UserPermissions");

            builder.HasKey(up => new { up.UserId, up.PermissionId });

            builder.Property(up => up.IsActive);
            builder.Property(up => up.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(up => up.UpdatedAt);

            builder.HasOne(up => up.User)
                   .WithMany(u => u.UserPermissions)
                   .HasForeignKey(up => up.UserId);

            builder.HasOne(up => up.Permission)
                   .WithMany(p => p.UserPermissions)
                   .HasForeignKey(up => up.PermissionId);
        }
    }

}
