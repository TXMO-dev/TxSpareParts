using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.Data;

namespace TxSpareParts.Infastructure.Repository
{
    public class ProductReviewRepository : Repository<ProductReview>, IProductReviewRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(ProductReview prod_rev)
        {
            _db.Update(prod_rev);
        }
    }
}
