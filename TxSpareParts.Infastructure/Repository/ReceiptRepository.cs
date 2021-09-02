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
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        private readonly ApplicationDbContext _db;
        public ReceiptRepository(ApplicationDbContext db):base(db) 
        {
            _db = db;
        }
        public void Update(Receipt receipt)
        {
            _db.Update(receipt);
        }
    }
}
