using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class CompanyImage
    {
        public string Id { get; set; }
        public string CompanyId { get; set; }
        public Company Company { get; set; }
        public string ImageUrl { get; set; }
    }
}
