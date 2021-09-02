using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility.interfaces
{
    public interface IInvoiceHandler
    {
        Task<string> GenerateReceipt(string userId, string refferencecode);
    }
}
