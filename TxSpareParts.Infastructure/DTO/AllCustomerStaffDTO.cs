using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class AllCustomerStaffDTO
    {
        public string ImageUrl { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhysicalAddress { get; set; }
        public string DigitalAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string PostOfficeBox { get; set; }
        public string PhoneNumber { get; set; }
    }
}
