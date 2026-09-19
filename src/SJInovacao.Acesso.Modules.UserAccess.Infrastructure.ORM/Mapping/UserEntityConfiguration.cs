using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Mapeia para uma tabela separada 'Users'
            builder.ToTable("Users");

            // Chave estrangeira para Persons (TPT)
            builder.HasBaseType<Person>();
            builder.Property(u => u.Username).HasMaxLength(50);
            builder.Property(u => u.Email).HasMaxLength(200);
            builder.Property(u => u.Password).HasMaxLength(255);
            builder.Property(u => u.Role).HasConversion<string>();
            builder.Property(u => u.Status).HasConversion<string>();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(u => u.UpdatedAt); 

            // Configurações para Refresh Token
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(200)      // GUID ou token JWT longo
                .IsRequired(false);     // pode ser nulo (usuário sem refresh token ativo)

            builder.Property(u => u.RefreshTokenExpiry)
                .IsRequired(false);     // pode ser nulo se não houver refresh token ativo

            // User <-> Permission (Many-to-Many)
            //builder.HasMany(u => u.Permissions)
            //    .WithMany(p => p.Users)
            //    .UsingEntity<Dictionary<string, object>>(
            //        "UserPermissions",
            //        j => j.HasOne<Permission>()
            //              .WithMany()
            //              .HasForeignKey("PermissionId")
            //              .OnDelete(DeleteBehavior.Cascade),
            //        j => j.HasOne<User>()
            //              .WithMany()
            //              .HasForeignKey("UserId")
            //              .OnDelete(DeleteBehavior.Cascade),
            //        j =>
            //        {
            //            j.Property<string>("Status").HasConversion<string>();
            //            j.Property<DateTime>("CreatedAt")
            //             .HasDefaultValueSql("CURRENT_TIMESTAMP");
            //            j.Property<DateTime?>("UpdatedAt");
            //        });

            // Relacionamento User <-> UserPermission (Many-to-Many explícito)
            builder.HasMany(u => u.UserPermissions)
                   .WithOne(up => up.User)
                   .HasForeignKey(up => up.UserId);




            // Relacionamento User -> UserGroup (one-to-many)
            builder.HasMany(u => u.UserGroups)
                   .WithOne(ug => ug.User)
                   .HasForeignKey(ug => ug.UserId);

        }
    }
}
