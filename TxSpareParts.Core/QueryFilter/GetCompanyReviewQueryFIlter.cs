﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.QueryFilter
{
    public class GetCompanyReviewQueryFIlter
    {
        public string companyId { get; set; }
        public PaginationQueryFIlter pagination { get; set; }
    }
}
