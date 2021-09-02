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
    public class CompanyReviewLikeRepository : Repository<CompanyReviewLike>, ICompanyReviewLikeRepository
    {
        private readonly ApplicationDbContext _db;
        public CompanyReviewLikeRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CompanyReviewLike like)
        {
            _db.Update(like);
        }
    }
}
