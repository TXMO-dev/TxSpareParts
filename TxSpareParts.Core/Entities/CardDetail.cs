using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class CardDetail
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string AccountName { get; set; }
        public string Email { get; set; }
        public string AuthorizationCode { get; set; }
        public string CardType { get; set; }
        public string last4 { get; set; }
        public string ExpMonth { get; set; }
        public string ExpYear { get; set; }
        public string Bin { get; set; }
        public string Bank { get; set; }
        public string PaymentChannel { get; set; }
        public string Signature { get; set; }
        public string CountryCode { get; set; }   
        public bool Reusable { get; set; }
        public bool SavedCard { get; set; }   
        public string RefferenceCode { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
