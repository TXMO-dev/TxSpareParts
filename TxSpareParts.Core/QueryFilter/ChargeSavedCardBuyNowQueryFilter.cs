using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class ChargeSavedCardBuyNowQueryFilter
    {
        public string userId { get; set; }
        public string productId { get; set; }
        public string signature { get; set; }
        public int quantity { get; set; }
    }
}
