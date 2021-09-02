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
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> entity)
        {
            entity.ToTable("Companies");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.Name)
                  .HasColumnName("Company Name")
                  .IsRequired();

            entity.Property(e => e.Email)
                  .HasColumnName("Company Email")
                  .IsRequired();

            entity.Property(e => e.PhoneNumber)
                  .HasColumnName("Company Phone Number")
                  .IsRequired();

            entity.Property(e => e.Address)
                  .HasColumnName("Company Physical Address")
                  .IsRequired();

            entity.Property(e => e.DigitalAddress)
                  .HasColumnName("Company Digital Address");

            entity.Property(e => e.PostOfficeBox)
                  .HasColumnName("Company PO Box");

            entity.Property(e => e.Description)
                  .HasColumnName("Company Description");

            entity.Property(e => e.IsVerified)
                  .HasColumnName("Is Verified?");

            entity.Property(e => e.AvgRating)
                  .HasColumnName("Average Rating");



        }
    }
}
