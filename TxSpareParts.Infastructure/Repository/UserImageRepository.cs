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
    public class UserImageRepository : Repository<UserImage>, IUserImageRepository
    {
        private readonly ApplicationDbContext _db;

        public UserImageRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(UserImage userimage)
        {
            _db.Update(userimage);
        }
    }
}
