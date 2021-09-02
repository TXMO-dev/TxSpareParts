using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class Receipt
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public int Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Currency { get; set; }
        public string ReferenceCode { get; set; }
        public string GatewayResponse { get; set; }
        public string Channel { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerPhoneNumber { get; set; }
        public string CustomerEmail { get; set; }
        public string CardType { get; set; }
        public string last4 { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Bin { get; set; }
        public string Bank { get; set; }
        public string CountryCode { get; set; }
        public int NumberOfItems { get; set; }  
        public string PurchasedItems { get; set; }    
        public string OrderNumber { get; set; }
        public int Total { get; set; }
    }
}
