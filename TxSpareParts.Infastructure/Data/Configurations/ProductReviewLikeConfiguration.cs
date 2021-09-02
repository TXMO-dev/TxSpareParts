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
    public class ProductReviewLikeConfiguration : IEntityTypeConfiguration<ProductReviewLike>
    {
        public void Configure(EntityTypeBuilder<ProductReviewLike> entity)
        {
            entity.ToTable("ProductReviewLikes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.ApplicationUser)
                .WithMany(e => e.ProductReviewLikes)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_userproductreviewlikes");

            entity.HasOne(e => e.Product)
                .WithMany(e => e.ProductReviewLikes)
                .HasForeignKey(e => e.ProductID)
                .HasConstraintName("FK_productproductreviewlikes");

            entity.HasOne(e => e.ProductReview)
                .WithMany(e => e.Likes)
                .HasForeignKey(e => e.ProductReviewID)
                .HasConstraintName("FK_theactualproductreview");
        }
    }
}
