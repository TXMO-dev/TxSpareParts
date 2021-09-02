using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class RegisterDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string DigitalAddress { get; set; }
        public string PhysicalAdress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Code { get; set; }
        public string role { get; set; }
    }
}
