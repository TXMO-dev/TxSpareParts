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
    public class ProductReviewLikeRepository : Repository<ProductReviewLike>, IProductReviewLikeRepository
    {
        private readonly ApplicationDbContext _db;
        public ProductReviewLikeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ProductReviewLike productreviewlike)
        {
            _db.Update(productreviewlike);
        }
    }
}
