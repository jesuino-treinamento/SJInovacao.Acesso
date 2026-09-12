using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SJInovacao.Acesso.Modules.UserAccess.Domain.Entities;

namespace SJInovacao.Acesso.Infrastructure.Data.Mappings
{
    public class ProductEntityConfiguration : IEntityTypeConfiguration<Product>
    {
        //public void Configure(EntityTypeBuilder<Product> builder)
        //{
        //    // Configuração da tabela
        //    builder.ToTable("Products");

        //    // Chave primária
        //    builder.HasKey(p => p.Id);
        //    builder.Property(u => u.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        //    // ProductConfiguration.cs (já enviado, apenas ajuste .WithMany)
        //    builder.HasOne(p => p.Customer)
        //           .WithMany(c => c.Products)  // se Customer tem coleção Products
        //           .HasForeignKey(p => p.CustomerId)
        //           .IsRequired(false);

        //    builder.HasOne(p => p.Catalog)
        //        .WithMany(c => c.Products)
        //        .HasForeignKey(p => p.CatalogId)
        //        .OnDelete(DeleteBehavior.Restrict)
        //        .IsRequired();

        //    // Branch
        //    builder.HasOne(p => p.Branch)
        //           .WithMany()
        //           .HasForeignKey(p => p.BranchId)
        //           .IsRequired();

        //    // Supplier (se usar)
        //    builder.HasOne(p => p.Supplier)
        //           .WithMany()
        //           .HasForeignKey(p => p.SupplierId)
        //           .IsRequired(false);


        //    builder.HasMany(p => p.Attachments)
        //        .WithOne()
        //        .HasForeignKey(a => a.ProductId)
        //        .OnDelete(DeleteBehavior.Cascade);

        //    // Configuração das propriedades
        //    builder.Property(p => p.Name)
        //         .IsRequired()
        //         .HasMaxLength(150)
        //         .HasColumnName("Name")
        //         .HasColumnType("varchar(150)");

        //    builder.Property(p => p.EAN)
        //        .HasColumnName("EAN")
        //        .HasColumnType("bigint");

        //    builder.Property(p => p.SKU)
        //        .IsRequired()
        //        .HasMaxLength(50)
        //        .HasColumnName("SKU")
        //        .HasColumnType("varchar(50)");

        //    builder.Property(p => p.Description)
        //        .IsRequired()
        //        .HasMaxLength(1000)
        //        .HasColumnName("Description")
        //        .HasColumnType("varchar(1000)");

        //    builder.Property(p => p.Price)
        //        .IsRequired()
        //        .HasColumnName("Price")
        //        .HasColumnType("decimal(18,2)");

        //    builder.Property(p => p.Size)
        //        .HasColumnName("Size")
        //        .HasColumnType("int");

        //    builder.Property(p => p.Weight)
        //        .HasColumnName("Weight")
        //        .HasColumnType("int");

        //    builder.Property(p => p.Color)
        //        .HasMaxLength(50)
        //        .HasColumnName("Color")
        //        .HasColumnType("varchar(50)");

        //    builder.Property(p => p.Measurement)
        //        .HasMaxLength(100)
        //        .HasColumnName("Measurement")
        //        .HasColumnType("varchar(100)");

        //    builder.Property(p => p.StockQuantity)
        //        .IsRequired()
        //        .HasColumnName("StockQuantity")
        //        .HasColumnType("int");

        //    builder.Property(p => p.Photo)
        //        .HasColumnName("Photo")
        //        .HasColumnType("bytea");

        //    builder.Property(p => p.IsActive)
        //        .IsRequired()
        //        .HasColumnName("IsActive")
        //        .HasColumnType("boolean")
        //        .HasDefaultValue(true);

        //    builder.HasIndex(p => p.EAN)
        //        .IsUnique()
        //        .HasDatabaseName("IX_Product_EAN");

        //    builder.HasIndex(p => p.SKU)
        //        .IsUnique()
        //        .HasDatabaseName("IX_Product_SKU");

        //    builder.HasIndex(p => p.Name)
        //        .HasDatabaseName("IX_Product_Name");

        //    builder.HasIndex(p => p.SubCatalogId)
        //        .HasDatabaseName("IX_Product_SubCatalog");

        //    builder.HasIndex(p => p.CatalogId)
        //        .HasDatabaseName("IX_Product_Catalog");
        //}

        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).HasMaxLength(200).IsRequired();
            builder.Property(p => p.Description).HasMaxLength(1000);
            builder.Property(p => p.Price).HasPrecision(18, 2);
            builder.Property(p => p.SKU).HasMaxLength(50);
            builder.Property(p => p.EAN).HasMaxLength(13);
            builder.Property(p => p.StockQuantity).IsRequired();

            builder.HasOne(p => p.SubCatalog)
                .WithMany() // SubCatalog não tem coleção de Products (mas pode ter se quiser)
                .HasForeignKey(p => p.SubCatalogId);

            builder.HasOne(p => p.Catalog)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CatalogId);

            builder.HasMany(p => p.Attachments)
                .WithOne(a => a.Product)
                .HasForeignKey(a => a.ProductId);

            builder.HasMany(p => p.Inventories)
                .WithOne(i => i.Product)
                .HasForeignKey(i => i.ProductId);
        }
    }
}