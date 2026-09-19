using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class PermissionEntityConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(p => p.Description)
                .HasMaxLength(250);

            builder.Property(u => u.IsActive).IsRequired();//.HasConversion<string>();
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(u => u.UpdatedAt);

            // Relacionamentos inversos (opcional, mas recomendado)
            builder.HasMany(p => p.UserPermissions)
                   .WithOne(up => up.Permission)
                   .HasForeignKey(up => up.PermissionId);
            
            // Analisar melhor para criar uma classe e tabela
            builder.HasMany(p => p.Groups)
                   .WithMany(g => g.Permissions)
                   .UsingEntity(j => j.ToTable("Groups_Permissions"));
        }
    }
}
