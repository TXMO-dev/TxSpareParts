using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public class ApplicationUser: IdentityUser
    {
        public ApplicationUser()
        {
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
            Likes = new HashSet<Like>();
            ProductReviews = new HashSet<ProductReview>();
            CompanyReviewLikes = new HashSet<CompanyReviewLike>();
            Starred = new HashSet<Star>();
            ShoppingCarts = new HashSet<ShoppingCart>();
            CardDetails = new HashSet<CardDetail>();
            Receipts = new HashSet<Receipt>();
            UserImages = new HashSet<UserImage>();
        }
      
        public string CompanyId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual Company Company { get; set; }
        public string EmployeeStatus { get; set; }
        public bool isVerified { get; set; }
        public string EmailForCard { get; set; }
        public string DigitalAddress {get; set;}
        public string PhysicalAdress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Password { get; set; }     
        public string Code { get; set; }
        public string AssignedTo { get; set; }
        public string AdministrativeStatus { get; set; }  
        public string SupervisorName { get; set; }
        public string role { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<ProductReview> ProductReviews { get; set; }
        public virtual ICollection<CompanyReview> CompanyReviews { get; set; }
        public virtual ICollection<ProductReviewLike> ProductReviewLikes { get; set; }  
        public virtual ICollection<CompanyReviewLike> CompanyReviewLikes { get; set; }
        public virtual ICollection<Star> Starred { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<CardDetail> CardDetails { get; set; }
        public virtual ICollection<Receipt> Receipts { get; set; }
        public virtual ICollection<UserImage> UserImages { get; set; }
        
    }
}
