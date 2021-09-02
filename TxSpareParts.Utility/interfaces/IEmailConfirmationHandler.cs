using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Utility.interfaces
{
    public interface IEmailConfirmationHandler
    {
        Task<string> GenerateToken(ApplicationUser User);
    }
}
