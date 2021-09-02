using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class ShoppingCart
    {
        public string ID { get; set; }
        public string orderID { get; set; }
        public string ProductID { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser ApplicationUser { get; set; }
        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
        public int Quantity { get; set; }
        public double Total { get; set; } 
    }
}
