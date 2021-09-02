using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class ProductReviewQueryFilter
    {
        public string productId { get; set; }
        public PaginationQueryFIlter filter { get; set; }
    }
}
