using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class ProductDTO
    {
        public string Name { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public int Quantity { get; set; }  
        public double Price { get; set; }
        public string Category { get; set; }
        public string Manufacturer { get; set; }  
        public string SupervisorName { get; set; }  
    }
}
