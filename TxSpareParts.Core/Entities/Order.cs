using System;
using System.Collections.Generic;

namespace TxSpareParts.Core.Entities
{
    public class Order
    {
        public Order()
        {
            ShoppingCarts = new HashSet<ShoppingCart>();
        }
        public string Id { get; set; }
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public string OrderStatus { get; set; }
        public string OrderNumber { get; set; }
        public string Carrier { get; set; }
        public string TrackingNumber { get; set; }
        public  DateTime OrderDate { get; set; }
        public string RefferenceCode { get; set; }
        public int OrderTotal { get; set; }
        public  bool PaidFor { get; set; }  
        public string Assignedto { get; set; }
        public string EstimatedDeliveryDate { get; set; }
    }
}