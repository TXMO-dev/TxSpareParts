using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Infastructure.Data.Configurations
{
    public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> entity)
        {
            entity.ToTable("ProductImages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.Product)
                .WithMany(e => e.ProductImages)
                .HasForeignKey(e => e.ProductId)
                .HasConstraintName("FK_productimagesforproduct");

            entity.Property(e => e.ImageUrl)
                .HasColumnName("Product Image")
                .IsRequired();    
        }
    }
}
