using System;
using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public class Product
    {
        public Product()
        {
            Likes = new HashSet<Like>();
        }
        public string Id { get; set; }  
        public string userID { get; set; }
        public string CompanyID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public string Name { get; set; } 
        public string Description { get; set; }
        public int Quantity { get; set; }
        public string Manufacturer { get; set; }     
        public double Price { get; set; }
        public Company Company { get; set; }
        public string Category { get; set; }
        public int NumberOfUpdates { get; set; }
        public string SupervisorName { get; set; }   
        public string recently_updated_by { get; set; }
        public string from_supervisor { get; set; }
        public virtual ICollection<Like> Likes { get; set; }
        public virtual ICollection<ProductReview> Reviews { get; set; }
        public virtual ICollection<ProductReviewLike> ProductReviewLikes { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public virtual ICollection<ProductImage> ProductImages { get; set; }
    }
}