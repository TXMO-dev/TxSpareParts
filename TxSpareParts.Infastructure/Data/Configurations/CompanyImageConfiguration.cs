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
    public class CompanyImageConfiguration : IEntityTypeConfiguration<CompanyImage>
    {
        public void Configure(EntityTypeBuilder<CompanyImage> entity)
        {
            entity.ToTable("CompanyImages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.Company)
                .WithMany(e => e.CompanyImages)
                .HasForeignKey(e => e.CompanyId)
                .HasConstraintName("FK_companyimagesofcompany");
        }
    }
}
