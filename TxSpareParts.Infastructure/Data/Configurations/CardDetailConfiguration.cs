using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Infastructure.Data.Configurations
{
    public class CardDetailConfiguration : IEntityTypeConfiguration<CardDetail>
    {
        public void Configure(EntityTypeBuilder<CardDetail> entity)
        {
            entity.ToTable("CardDetails");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany(e => e.CardDetails)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_carddetailapplicationuser");

            entity.Property(e => e.AccountName)
                .HasColumnName("Account Name");

            entity.Property(e => e.Email)
                .HasColumnName("transaction Email");

            entity.Property(e => e.AuthorizationCode)
                .HasColumnName("Authorization Code");

            entity.Property(e => e.CardType)
                .HasColumnName("Card Type");

            entity.Property(e => e.last4)
                .HasColumnName("Last 4 digits of card");

            entity.Property(e => e.ExpMonth)
                .HasColumnName("Expiry Month");

            entity.Property(e => e.ExpYear)
                .HasColumnName("Expiry Year");

            entity.Property(e => e.SavedCard)
                .HasColumnName("Is Card Saved"); 


                
        }
    }
}
