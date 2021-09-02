using Gehtsoft.PDFFlow.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility
{
    public class ProductReceipt
    {
        public ImageBuilder ProductImage { get; set; }
        public string CompanyName { get; set; }
        public string ProductName { get; set; }
        public string Manufacturer { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public int Total { get; set; }   
    }
}
