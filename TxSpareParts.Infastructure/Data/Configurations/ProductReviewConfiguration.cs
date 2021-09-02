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
    public class ProductReviewConfiguration : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> entity)
        {
            entity.ToTable("ProductReviews");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();  

            //entity.Ignore(e => e.User);

            entity.Property(e => e.Comment)
                  .HasColumnName("Reviews")
                  .IsRequired();

            entity.Property(e => e.Rating)
                  .HasColumnName("Company Rating");


            entity.HasOne(e => e.Product)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.ProductID)
                  .HasConstraintName("FK_reviewofproducts");

            entity.HasOne(e => e.User)
                .WithMany(e => e.ProductReviews)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_applicationuserproductreview");
                  
                  

        }
    }
}
