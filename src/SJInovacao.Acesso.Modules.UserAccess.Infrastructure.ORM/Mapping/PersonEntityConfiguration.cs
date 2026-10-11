using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Modules.UserAccess.Infrastructure.ORM.Mapping
{
    public class PersonEntityConfiguration : IEntityTypeConfiguration<Person>
    {
        public void Configure(EntityTypeBuilder<Person> builder)
        {
            builder.ToTable("Persons");
            builder.HasKey(p => p.Id);
            builder.OwnsOne(p => p.Name, n =>
            {
                n.Property(nn => nn.FirstName).HasColumnName("FirstName").HasMaxLength(100);
                n.Property(nn => nn.LastName).HasColumnName("LastName").HasMaxLength(100);
            });
            builder.OwnsOne(p => p.Document, d =>
            {
                d.Property(doc => doc.Number).HasColumnName("DocumentNumber").HasMaxLength(18);
                d.Property(doc => doc.PersonType).HasColumnName("PersonType").HasConversion<string>(); 
            });           
        }
    }
}
