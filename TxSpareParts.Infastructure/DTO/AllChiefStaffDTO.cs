using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Infastructure.DTO
{
    public class AllChiefStaffDTO
    {
        public string CompanyName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmployeeStatus { get; set; }
        public string PhoneNumber { get; set; }
        public string StaffUnderSupervisorFirstName { get; set; }
        public string StaffUnderSupervisorLastName { get; set; } 
        public bool supervisorsonly { get; set; }
        public bool staffonly { get; set; }
        public bool administratorsonly { get; set; }
    }
}
