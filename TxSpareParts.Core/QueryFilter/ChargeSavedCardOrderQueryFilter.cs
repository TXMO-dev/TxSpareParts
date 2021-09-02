using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class ChargeSavedCardOrderQueryFilter
    {
        public string userId { get; set; }
        public string orderId { get; set; }
        public string signature { get; set; }   
    }
}
