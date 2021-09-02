using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class ResetPasswordQueryFilter 
    {
        public string email { get; set; }
        public string token { get; set; }
    }
}
