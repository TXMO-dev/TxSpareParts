using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class HandleMultipleSplitResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Subaccount2
        {
            public int id { get; set; }
            public string subaccount_code { get; set; }
            public string business_name { get; set; }
            public string description { get; set; }
            public object primary_contact_name { get; set; }
            public object primary_contact_email { get; set; }
            public object primary_contact_phone { get; set; }
            public object metadata { get; set; }
            public int percentage_charge { get; set; }
            public string settlement_bank { get; set; }
            public string account_number { get; set; }
        }

        public class Subaccount
        {
            public Subaccount2 subaccount { get; set; }   
            public int share { get; set; }
        }

        public class Data
        {
            public int id { get; set; }
            public string name { get; set; }
            public string type { get; set; }
            public string currency { get; set; }
            public int integration { get; set; }
            public string domain { get; set; }
            public string split_code { get; set; }
            public bool active { get; set; }
            public string bearer_type { get; set; }
            public int bearer_subaccount { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public List<Subaccount> subaccounts { get; set; }
            public int total_subaccounts { get; set; }
        }

            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        


    }
}
