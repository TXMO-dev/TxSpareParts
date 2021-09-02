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
    public class LikeRepository : Repository<Like>, ILikeRepository
    {
        private readonly ApplicationDbContext _db;
        public LikeRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;
        }

        public void Update(Like like)
        {
            _db.Update(like);
        }
    }
}
