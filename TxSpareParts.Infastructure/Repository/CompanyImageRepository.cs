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
    public class CompanyImageRepository : Repository<CompanyImage>, ICompanyImageRepository
    {
        private readonly ApplicationDbContext _db;

        public CompanyImageRepository(ApplicationDbContext db) : base(db)  
        {
            _db = db;
        }
        public void Update(CompanyImage companyimage)
        {
            _db.Update(companyimage);
        }
    }
}
