//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata.Builders;
//using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

//namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
//{
//    public class UserGroupEntityConfiguration : IEntityTypeConfiguration<UserGroup>
//    {
//        public void Configure(EntityTypeBuilder<UserGroup> builder)
//        {
//            builder.ToTable("UserGroups");
//            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

//            builder.HasOne(ug => ug.User) 
//                   .WithMany(u => u.UserGroups)
//                   .HasForeignKey(ug => ug.UserId);

//            builder.HasOne(ug => ug.Group)
//              .WithMany(g => g.UserGroups)
//              .HasForeignKey(ug => ug.GroupId);

//            builder.Property(ug => ug.IsActive)
//                   .IsRequired()
//                   .HasDefaultValue(true);

//            builder.Property(ug => ug.AssignedAt)
//                   .HasDefaultValueSql("CURRENT_TIMESTAMP");
//        }
//    }
//}

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

            // Chave primária composta — já cria índice (UserId, GroupId)
            builder.HasKey(ug => new { ug.UserId, ug.GroupId });

            // Relacionamentos
            builder.HasOne(ug => ug.User)
                   .WithMany(u => u.UserGroups)
                   .HasForeignKey(ug => ug.UserId);

            builder.HasOne(ug => ug.Group)
                   .WithMany(g => g.UserGroups)
                   .HasForeignKey(ug => ug.GroupId);

            // Propriedades — SEM MUDANÇA
            builder.Property(ug => ug.IsActive)
                   .IsRequired()
                   .HasDefaultValue(true);

            builder.Property(ug => ug.AssignedAt)
                   .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // =========================
            // ÍNDICES (NÃO criam colunas)
            // =========================
            // A PK já indexa (UserId, GroupId).
            // Estes índices são APENAS estruturas de busca adicionais
            // para consultas que filtram/ordenam por GroupId sozinho.

            builder.HasIndex(ug => ug.GroupId)
                   .HasDatabaseName("IX_UserGroups_GroupId");
        }
    }
}
