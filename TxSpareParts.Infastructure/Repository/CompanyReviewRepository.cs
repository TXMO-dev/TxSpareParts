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
    public class CompanyReviewRepository : Repository<CompanyReview>, ICompanyReviewRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyReviewRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CompanyReview compreview)
        {
            _db.Update(compreview);
        }
    }
}
