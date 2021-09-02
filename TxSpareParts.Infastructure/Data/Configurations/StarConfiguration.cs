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
    class StarConfiguration : IEntityTypeConfiguration<Star>
    {
        public void Configure(EntityTypeBuilder<Star> entity)
        {
            entity.ToTable("Stars");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Starred)
                  .HasForeignKey(e => e.CompanyID)
                  .HasConstraintName("FK_companystar");

            entity.HasOne(e => e.ApplicationUser)
                .WithMany(e => e.Starred)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_alluserstarred");
        }
    }
}
