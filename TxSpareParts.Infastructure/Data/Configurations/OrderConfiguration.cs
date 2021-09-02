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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OrderStatus)
                  .HasColumnName("Order Status");

            entity.Property(e => e.OrderNumber)
                  .HasColumnName("Order Number");

            entity.Property(e => e.TrackingNumber)
                  .HasColumnName("Tracking Number");

            entity.Property(e => e.OrderDate)
                  .HasColumnName("Order Date")
                  .HasColumnType("datetime")
                  .IsRequired();

            entity.HasOne(e => e.User)
                .WithMany(e => e.Orders)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_userofshoppingcart");

            entity.Property(e => e.PaidFor)
                .HasColumnName("Paid For?")   
                .HasDefaultValue(false)
                .IsRequired();

            entity.Property(e => e.Assignedto)
                .HasColumnName("Hendled By")
                .IsRequired();

        }
    }
}
