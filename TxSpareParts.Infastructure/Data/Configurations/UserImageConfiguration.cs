using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Infastructure.Data.Configurations
{
    public class UserImageConfiguration : IEntityTypeConfiguration<UserImage>
    {
        public void Configure(EntityTypeBuilder<UserImage> entity)
        {
            entity.ToTable("UserImages");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            entity.HasOne(e => e.User)
                .WithMany(e => e.UserImages)
                .HasForeignKey(e => e.UserId)
                .HasConstraintName("FK_userimagesconstrains");

            entity.Property(e => e.ImageUrl)
                .IsRequired();  
        }
    }
}
