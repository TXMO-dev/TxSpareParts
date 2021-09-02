using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class CompanyDTO 
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string CompanyImage { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string DigitalAddress { get; set; }
        public string PostOfficeBox  { get; set; }
        public string SecondPhoneNumber { get; set; }
        public string ThirdPhoneNumber { get; set; }
        public string AccountNumber { get; set; }
        public string SupportedBank { get; set; }
    }
}
