using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class UserGroupEntityConfiguration : IEntityTypeConfiguration<UserGroup>
    {
        public void Configure(EntityTypeBuilder<UserGroup> builder)
        {
            // 1. Define a tabela e a chave primária composta
            builder.ToTable("UserGroups");
            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

            // 2. Configura o relacionamento com User
            builder.HasOne<User>(ug => ug.User) // Especifica o tipo da entidade relacionada
                   .WithMany(u => u.UserGroups) // Usa a navegação CORRETA em User
                   .HasForeignKey(ug => ug.UserId)
                   .OnDelete(DeleteBehavior.Restrict); // Recomendo usar Restrict ou ClientCascade para evitar loops

            // 3. Configura o relacionamento com GroupPermission
            builder.HasOne<GroupPermission>(ug => ug.Group) // Especifica o tipo da entidade relacionada
                   .WithMany(g => g.UserGroups) // Usa a navegação CORRETA em GroupPermission
                   .HasForeignKey(ug => ug.GroupId)
                   .OnDelete(DeleteBehavior.Restrict);

            // 4. Configura as propriedades adicionais
            builder.Property(ug => ug.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(ug => ug.AssignedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}