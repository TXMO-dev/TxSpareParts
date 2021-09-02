using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class CompanyReviewLike
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public string CompanyID { get; set; }
        public string CompanyReviewID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Company Company { get; set; }
        public virtual CompanyReview CompanyReview { get; set; }
    }
}
