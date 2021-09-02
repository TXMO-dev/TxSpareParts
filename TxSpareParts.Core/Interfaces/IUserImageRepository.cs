using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Core.Interfaces
{
    public interface IUserImageRepository : IRepository<UserImage>
    {
        void Update(UserImage userimage);
    }
}
