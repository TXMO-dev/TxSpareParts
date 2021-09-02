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
    public class LikeConfiguration : IEntityTypeConfiguration<Like>
    {
        public void Configure(EntityTypeBuilder<Like> entity)
        {
            entity.ToTable("Likes");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Ignore(e => e.ApplicationUser);

            entity.HasOne(e => e.Product)
                  .WithMany(e => e.Likes)
                  .HasForeignKey(e => e.productID)
                  .HasConstraintName("FK_likeofproduct");

            entity.HasOne(e => e.ApplicationUser)
                  .WithMany(e => e.Likes)
                  .HasForeignKey(e => e.userID)
                  .HasConstraintName("FK_applicationuserlike");



        }
    }
}
