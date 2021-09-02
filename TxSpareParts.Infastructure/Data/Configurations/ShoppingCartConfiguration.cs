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
    public class ShoppingCartConfiguration : IEntityTypeConfiguration<ShoppingCart>
    {
        public void Configure(EntityTypeBuilder<ShoppingCart> entity)
        {
            entity.ToTable("ShoppingCarts");
            entity.HasKey(e => e.ID);

            entity.Property(e => e.ID)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.ApplicationUser)
                .WithMany(e => e.ShoppingCarts)
                .HasForeignKey(e => e.UserID)
                .HasConstraintName("FK_shoppingcartappuser");

            entity.HasOne(e => e.Order)
                  .WithMany(e => e.ShoppingCarts)
                  .HasForeignKey(e => e.orderID)
                  .HasConstraintName("FK_ordershoppingcart");

            entity.HasOne(e => e.Product)
                .WithMany(e => e.ShoppingCarts)
                .HasForeignKey(e => e.ProductID)
                .HasConstraintName("FK_shoppingcartforproduct");

            entity.Property(e => e.Total)
                .HasColumnName("Cart Total")     
                .IsRequired();  
        }
    }
}
