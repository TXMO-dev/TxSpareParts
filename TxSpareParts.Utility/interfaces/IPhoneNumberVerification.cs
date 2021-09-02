using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Utility.interfaces
{
    public interface IPhoneNumberVerification
    {
        Task<string> SendToken(string phonenumber);
        Task<string> VerifyToken(ApplicationUser user,string code);
    }
}
