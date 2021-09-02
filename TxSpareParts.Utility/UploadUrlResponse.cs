using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class UploadUrlResponse
    {
         public class Data
        {
            public string signedUrl { get; set; }
            public string fileName { get; set; }
        }

        
            public bool status { get; set; }
            public string message { get; set; }
            public Data data { get; set; }
        
    }
}
