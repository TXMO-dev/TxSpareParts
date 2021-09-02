using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Infastructure.DTO
{
    public class OrderDTO
    {
        public OrderDTO()
        {
            ShoppingCarts = new HashSet<ShoppingCart>();
        }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public string OrderStatus { get; set; }
        public string OrderNumber { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public string EstimatedDeliveryDate { get; set; }      
    }
}
