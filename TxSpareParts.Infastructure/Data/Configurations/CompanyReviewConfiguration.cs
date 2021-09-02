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
    public class CompanyReviewConfiguration : IEntityTypeConfiguration<CompanyReview>
    {
        public void Configure(EntityTypeBuilder<CompanyReview> entity)
        {
            entity.ToTable("CompanyReviews");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            //entity.Ignore(e => e.User);

            entity.Property(e => e.Comment)
                  .HasColumnName("Reviews")
                  .IsRequired();

            entity.Property(e => e.Rating)
                  .HasColumnName("Company Rating");      


            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Reviews)
                  .HasForeignKey(e => e.CompanyID)
                  .HasConstraintName("FK_reviewofcompanies");

            entity.HasOne(e => e.User)
                .WithMany(e => e.CompanyReviews)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_companyreviewsinuser");



        }
    }
}
