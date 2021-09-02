using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public class Company
    {
        public Company()
        {
            Products = new HashSet<Product>();
            Reviews = new HashSet<CompanyReview>();
            CompanyReviewLikes = new HashSet<CompanyReviewLike>();
            CompanyImages = new HashSet<CompanyImage>();
            Starred = new HashSet<Star>();   
        }

        public string ID { get; set; }    
        public virtual ICollection<ApplicationUser> Employees { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string DigitalAddress {get; set;}
        public string PostOfficeBox { get; set; }
        public string Description { get; set; }
        public bool IsVerified { get; set; }
        public double AvgRating { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string ThirdPhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string SupportedBank { get; set; }
        public virtual ICollection<Product> Products { get; set; } 
        public virtual ICollection<CompanyReview> Reviews { get; set; }
        public virtual ICollection<CompanyReviewLike> CompanyReviewLikes { get; set; }
        public virtual ICollection<CompanyImage> CompanyImages { get; set; }
        public ICollection<Star> Starred { get; set; }   
    }
}