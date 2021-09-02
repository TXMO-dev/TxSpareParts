using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Infastructure.Data.Configurations;

namespace TxSpareParts.Infastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        private readonly DbContextOptions _options;
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
            _options = options;
        }

        //public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }
        public DbSet<CompanyReview> CompanyReviews { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Star> Stars { get; set; }  
        public DbSet<ProductReviewLike> ProductReviewLikes { get; set; } 
        public DbSet<CompanyReviewLike> CompanyReviewLikes { get; set; }
        public DbSet<CardDetail> CardDetails { get; set; }
        public DbSet<Receipt> Receipts { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<CompanyImage> CompanyImages { get; set; }
        public DbSet<UserImage> UserImages { get; set; }      

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

}
