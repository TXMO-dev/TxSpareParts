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
    public class CardDetailRepository : Repository<CardDetail>, ICardDetailRepository
    {
        private readonly ApplicationDbContext _db;

        public CardDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(CardDetail carddetail)
        {
            _db.Update(carddetail);
        }
    }
}
