using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasBaseType<Person>();
            builder.Property(u => u.Username).HasMaxLength(50);
            builder.Property(u => u.Email).HasMaxLength(200);
            builder.Property(u => u.Password).HasMaxLength(255);
            builder.Property(u => u.Role).HasConversion<string>();
            builder.Property(u => u.Status).HasConversion<string>();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(u => u.UpdatedAt); 

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(200)      
                .IsRequired(false);     

            builder.Property(u => u.RefreshTokenExpiry)
                .IsRequired(false);     
                        
            builder.HasMany(u => u.UserPermissions)
                   .WithOne(up => up.User)
                   .HasForeignKey(up => up.UserId);

            builder.HasMany(u => u.UserGroups)
                   .WithOne(ug => ug.User)
                   .HasForeignKey(ug => ug.UserId);     
        }
    }
}
