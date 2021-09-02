using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class ProductImage
    {
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ImageUrl { get; set; }
        public virtual Product Product { get; set; }
    }
}
