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
    public class CompanyReviewLikeConfiguration : IEntityTypeConfiguration<CompanyReviewLike>
    {
        public void Configure(EntityTypeBuilder<CompanyReviewLike> entity)
        {
            entity.ToTable("CompanyReviewLikes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.ApplicationUser)
                .WithMany(e => e.CompanyReviewLikes)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_usercompanyreviewlikes");

            entity.HasOne(e => e.Company)
                .WithMany(e => e.CompanyReviewLikes)
                .HasForeignKey(e => e.CompanyID)
                .HasConstraintName("FK_companycompanyreviewlike");

            entity.HasOne(e => e.CompanyReview)
                .WithMany(e => e.CompanyReviewLikes)
                .HasForeignKey(e => e.CompanyReviewID)
                .HasConstraintName("FK_companyreviewcomplikes");  
        }
    }
}
