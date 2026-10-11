using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UserGroupEntityConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            builder.ToTable("UserGroups");

            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

            builder.HasOne(ug => ug.User)
                   .WithMany(u => u.UserGroups)
                   .HasForeignKey(ug => ug.UserId);

            builder.HasOne(ug => ug.Group)
                   .WithMany(g => g.UserGroups)
                   .HasForeignKey(ug => ug.GroupId);

            builder.Property(ug => ug.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(ug => ug.AssignedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            builder.HasIndex(ug => ug.GroupId)
                   .HasDatabaseName("IX_UserGroups_GroupId");
        }
    }
}
