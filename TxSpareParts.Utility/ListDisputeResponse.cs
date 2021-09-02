using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class ListDisputeResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Authorization
        {
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
            public object customer { get; set; }
            public Plan plan { get; set; }
            public Subaccount subaccount { get; set; }
            public Split split { get; set; }
            public object order_id { get; set; }
            public DateTime paidAt { get; set; }
            public DateTime createdAt { get; set; }
            public object pos_transaction_data { get; set; }
        }

        public class Customer
        {
            public int id { get; set; }
            public object first_name { get; set; }
            public object last_name { get; set; }
            public string email { get; set; }
            public string customer_code { get; set; }
            public object phone { get; set; }
            public object metadata { get; set; }
            public string risk_action { get; set; }
            public object international_format_phone { get; set; }
        }

        public class History
        {
            public string status { get; set; }
            public string by { get; set; }
            public DateTime createdAt { get; set; }
        }

        public class Message
        {
            public string sender { get; set; }
            public string body { get; set; }
            public DateTime createdAt { get; set; }
        }

        public class Data
        {
            public int id { get; set; }
            public object refund_amount { get; set; }
            public object currency { get; set; }
            public string status { get; set; }
            public object resolution { get; set; }
            public string domain { get; set; }
            public Transaction transaction { get; set; }
            public object transaction_reference { get; set; }
            public object category { get; set; }
            public Customer customer { get; set; }
            public object bin { get; set; }
            public object last4 { get; set; }
            public object dueAt { get; set; }
            public object resolvedAt { get; set; }
            public object evidence { get; set; }
            public string attachments { get; set; }
            public object note { get; set; }
            public List<History> history { get; set; }
            public List<Message> messages { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

        public class Meta
        {
            public int total { get; set; }
            public int skipped { get; set; }
            public int perPage { get; set; }
            public int page { get; set; }
            public int pageCount { get; set; }
        }

       
            public bool status { get; set; }
            public string message { get; set; }
            public List<Data> data { get; set; }
            public Meta meta { get; set; }
        


    }
}
