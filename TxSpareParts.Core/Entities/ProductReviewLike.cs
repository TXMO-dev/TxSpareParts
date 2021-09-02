using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class ProductReviewLike
    {
        public string Id { get; set; }   
        public string UserID { get; set; }
        public string ProductID { get; set; }
        public string ProductReviewID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductReview ProductReview { get; set; }
    }
}
