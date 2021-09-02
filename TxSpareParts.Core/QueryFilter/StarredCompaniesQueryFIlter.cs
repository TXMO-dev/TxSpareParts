using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class StarredCompaniesQueryFIlter
    {
        public string userId { get; set; }
        public PaginationQueryFIlter pagination { get; set; }
    }
}
   