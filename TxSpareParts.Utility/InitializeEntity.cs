using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class InitializeEntity
    {
        public string email { get; set; }
        public string amount { get; set; }
        public string currency { get; set; }
        public string refference { get; set; }
        public string callback_url { get; set; }
        public string[] channels { get; set; }
        public string split_code { get; set; }  
    }
}
