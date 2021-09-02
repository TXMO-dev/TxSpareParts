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
    public class ReceiptConfiguration : IEntityTypeConfiguration<Receipt>
    {
        public void Configure(EntityTypeBuilder<Receipt> entity)
        {
            entity.ToTable("Receipts");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany(e => e.Receipts)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_usersreceiptname");

            entity.Property(e => e.Amount)
                .IsRequired();

            entity.Property(e => e.Currency)
                .IsRequired();

            entity.Property(e => e.ReferenceCode)
                .IsRequired();

            entity.Property(e => e.GatewayResponse)
                .IsRequired();

            entity.Property(e => e.Channel)
                .IsRequired();

            entity.Property(e => e.CustomerFirstName)
                .IsRequired();

            entity.Property(e => e.CustomerLastName)
                .IsRequired();

            entity.Property(e => e.CustomerPhoneNumber)
                .IsRequired();

            entity.Property(e => e.CustomerEmail)
                .IsRequired();


        }
    }
}
