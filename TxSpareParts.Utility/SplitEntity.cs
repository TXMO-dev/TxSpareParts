using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    
    public class SplitEntity
    {
            public string name { get; set; }
            public string type { get; set; }
            public string currency { get; set; }
            public List<SubAccountEntity> subaccounts { get; set; }
            public string bearer_type { get; set; }
            public string bearer_subaccount { get; set; }
    }
}
