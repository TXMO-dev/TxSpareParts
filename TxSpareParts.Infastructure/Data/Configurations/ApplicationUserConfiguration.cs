using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Infastructure.Data.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> entity)
        {
            entity.ToTable("AspNetUsers");


            entity.Property(e => e.FirstName)
                  .HasColumnName("First Name");

            entity.Property(e => e.LastName)
                  .HasColumnName("Last Name");

            entity.Property(e => e.EmployeeStatus)
                  .HasColumnName("Employee Status")
                  .HasMaxLength(12);

            entity.Property(e => e.isVerified)
                  .HasColumnName("Is Verified");

            entity.Property(e => e.DigitalAddress)
                  .HasColumnName("Digital Address")
                  .HasMaxLength(12);

            entity.Property(e => e.PhysicalAdress)
                  .HasColumnName("Physical Address");

            entity.HasOne(e => e.Company)
                  .WithMany(e => e.Employees)
                  .HasForeignKey(e => e.CompanyId)
                  .HasConstraintName("FK_companyapplicationuser");

            entity.Property(e => e.AdministrativeStatus)
                .HasColumnName("Administrative Status")
                .HasMaxLength(12);

            entity.Property(e => e.AssignedTo)
                .HasColumnName("Assigned To?");


            entity.Ignore(e => e.Password);
            entity.Ignore(e => e.Code);
            entity.Ignore(e => e.SupervisorName);
            entity.Ignore(e => e.role);
        }
    }
}
