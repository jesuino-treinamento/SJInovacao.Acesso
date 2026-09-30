using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    //public class GroupsPermissionsEntityConfiguration : IEntityTypeConfiguration<GroupsPermissions>
    //{
    //    public void Configure(EntityTypeBuilder<GroupsPermissions> builder)
    //    {
    //        // Nome da tabela
    //        builder.ToTable("GroupsPermissions");

    //        // Chave composta: Group + Permission
    //        builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

    //        // Relacionamento com GroupPermission
    //        //builder.HasOne(gp => gp.Group)
    //        //       .WithMany(g => g.GroupsPermissions)
    //        //       .HasForeignKey(gp => gp.GroupId);

    //        builder.HasOne<GroupPermission>(ug => ug.Group) // Especifica o tipo da entidade relacionada
    //               .WithMany(g => g.GroupsPermissions) // Usa a navegação CORRETA em GroupPermission
    //               .HasForeignKey(ug => ug.GroupId)
    //               .OnDelete(DeleteBehavior.Restrict);

    //        // Relacionamento com Permission
    //        builder.HasOne<Permission>(gp => gp.Permission)
    //               .WithMany(p => p.GroupsPermissions)
    //               .HasForeignKey(gp => gp.PermissionId);

    //        // Propriedades adicionais
    //        builder.Property(gp => gp.IsActive).IsRequired();
    //        builder.Property(gp => gp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
    //        builder.Property(gp => gp.UpdatedAt);
    //    }
    //}

    public class GroupsPermissionsEntityConfiguration : IEntityTypeConfiguration<GroupsPermissions>
    {
        public void Configure(EntityTypeBuilder<GroupsPermissions> builder)
        {
            builder.ToTable("GroupsPermissions");

            // Chave composta: Group + Permission
            builder.HasKey(gp => new { gp.GroupId, gp.PermissionId });

            // Relacionamento com GroupPermission
            builder.HasOne(gp => gp.Group)
                   .WithMany(g => g.GroupsPermissions)
                   .HasForeignKey(gp => gp.GroupId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relacionamento com Permission
            builder.HasOne(gp => gp.Permission)
                   .WithMany(p => p.GroupsPermissions)
                   .HasForeignKey(gp => gp.PermissionId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Propriedades adicionais
            builder.Property(gp => gp.IsActive).IsRequired();
            builder.Property(gp => gp.CreatedAt).HasDefaultValueSql("CURRENT_TIMESTAMP");
            builder.Property(gp => gp.UpdatedAt);
        }
    }

}
