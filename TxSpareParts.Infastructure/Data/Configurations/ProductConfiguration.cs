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
    class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.ToTable("Products");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                  .ValueGeneratedOnAdd();  

            entity.HasOne(e => e.ApplicationUser)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.userID);
                

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Products)
                  .HasForeignKey(e => e.CompanyID)
                  .HasConstraintName("FK_compproduct");

            entity.Property(e => e.Description)
                  .HasColumnName("Product Description")
                  .IsRequired();

            entity.Property(e => e.Quantity)
                  .HasColumnName("Product Quantity")
                  .HasMaxLength(200)
                  .IsRequired();

            entity.Property(e => e.Price)
                  .HasColumnName("Product Price")
                  .IsRequired();

            entity.Property(e => e.Name)
                .HasColumnName("Product Name")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.recently_updated_by)
                .HasColumnName("recently updated by")
                .IsRequired();

            entity.Property(e => e.Category)
                .HasColumnName("Product Category")
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(e => e.NumberOfUpdates)
                .HasColumnName("Number of Times Updated")
                .HasDefaultValue(0)
                .IsRequired();

            entity.Property(e => e.from_supervisor)
                .HasColumnName("From Supervisor");

            entity.Property(e => e.Manufacturer)
                .HasColumnName("Company Manufacturer");  

            entity.Ignore(e => e.SupervisorName);

        }
    }
}
