

using System;
using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public partial class ProductReview
    {
        public ProductReview()
        {
            Likes = new HashSet<ProductReviewLike>();
        }
        
        public string Id { get; set; }
        //public int CompanyID { get; set; }
        public string ProductID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public DateTime Date { get; set; }
        public DateTime Date_Updated { get; set; }
        //public virtual Company Company { get; set; }
        public virtual Product Product { get; set; }
        public virtual ICollection<ProductReviewLike> Likes { get; set; }        

      
    }
}