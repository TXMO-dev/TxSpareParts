using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class ResolveDisputeResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Authorization
        {
        }

        public class Customer
        {
            public object international_format_phone { get; set; }
        }

        public class Plan
        {
        }

        public class Subaccount
        {
        }

        public class Split
        {
        }

        public class Transaction
        {
            public int id { get; set; }
            public string domain { get; set; }
            public string status { get; set; }
            public string reference { get; set; }
            public int amount { get; set; }
            public object message { get; set; }
            public string gateway_response { get; set; }
            public DateTime paid_at { get; set; }
            public DateTime created_at { get; set; }
            public string channel { get; set; }
            public string currency { get; set; }
            public object ip_address { get; set; }
            public string metadata { get; set; }
            public object log { get; set; }
            public int fees { get; set; }
            public object fees_split { get; set; }
            public Authorization authorization { get; set; }
            public Customer customer { get; set; }
            public Plan plan { get; set; }
            public Subaccount subaccount { get; set; }
            public Split split { get; set; }
            public object order_id { get; set; }
            public DateTime paidAt { get; set; }
            public DateTime createdAt { get; set; }
            public object requested_amount { get; set; }
        }

        public class Message
        {
            public int dispute { get; set; }
            public string sender { get; set; }
            public string body { get; set; }
            public int id { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

        public class Data
        {
            public string currency { get; set; }
            public object last4 { get; set; }
            public object bin { get; set; }
            public object transaction_reference { get; set; }
            public object merchant_transaction_reference { get; set; }
            public int refund_amount { get; set; }
            public string status { get; set; }
            public string domain { get; set; }
            public string resolution { get; set; }
            public string category { get; set; }
            public object note { get; set; }
            public string attachments { get; set; }
            public int id { get; set; }
            public Transaction transaction { get; set; }
            public int created_by { get; set; }
            public object evidence { get; set; }
            public DateTime resolvedAt { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public object dueAt { get; set; }
            public Message message { get; set; }
        }

       
            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        


    }
}
