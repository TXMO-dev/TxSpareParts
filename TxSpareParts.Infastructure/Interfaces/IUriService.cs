using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.QueryFilter;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IUriService
    {
        Uri GetProductPaginationUri(PaginationQueryFIlter filter, string actionUrl);
    }
}
