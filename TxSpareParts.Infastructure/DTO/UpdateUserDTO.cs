using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class UpdateUserDTO
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string PhysicalAddress { get; set; }
        public string DigitalAddress { get; set; }
        public string City { get; set; }
        public string Region { get; set; }
        public string ImageUrl { get; set; }
        public string CurrentPassword { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmNewPassword { get; set; }
        public string EmployeeStatus { get; set; }
        public string AdministrativeStatus { get; set; }  
    }
}
