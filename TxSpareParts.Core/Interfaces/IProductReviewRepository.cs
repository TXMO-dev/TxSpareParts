using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Core.Interfaces
{
    public interface IProductReviewRepository : IRepository<ProductReview>
    {
        void Update(ProductReview prod_rev);
    }
}
