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
    public class StarRepository : Repository<Star>, IStarRepository
    {
        private readonly ApplicationDbContext _db;

        public StarRepository(ApplicationDbContext db) :base(db)
        {
            _db = db;

        }

        public void Update(Star star)
        {
            _db.Update(star);
        }
    }
}
