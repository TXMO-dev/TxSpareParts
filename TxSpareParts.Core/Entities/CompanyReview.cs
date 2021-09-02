using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public class CompanyReview
    {
        public CompanyReview()
        {
            CompanyReviewLikes = new HashSet<CompanyReviewLike>();
        }
        public string Id { get; set; }
        public string CompanyID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<CompanyReviewLike> CompanyReviewLikes { get; set; }
    }
}