using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class EvidenceResponse
    {
        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Data
        {
            public string customer_email { get; set; }
            public string customer_name { get; set; }
            public string customer_phone { get; set; }
            public string service_details { get; set; }
            public string delivery_address { get; set; }
            public int dispute { get; set; }
            public int id { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }

        
            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        
    }
}
