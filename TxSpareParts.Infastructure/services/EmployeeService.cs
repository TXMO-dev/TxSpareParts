using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Utility;

namespace TxSpareParts.Infastructure.services
{
    public class EmployeeService : IEmployeeService 
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IUnitOfWork _unitofwork;
        public EmployeeService(
            UserManager<ApplicationUser> usermanager,
            IUnitOfWork unitofwork)
        {
            _usermanager = usermanager;
            _unitofwork = unitofwork;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllSupervisorStaff(AllSupervisorDTO User, string Id)
        {
            var user = await _usermanager.FindByIdAsync(Id);
            if (user.EmployeeStatus == SD.Supervisor)
            {
                var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
                IList<ApplicationUser> staff_list = new List<ApplicationUser>(); ;
                IList<ApplicationUser> logged_in_supervisor_staff = new List<ApplicationUser>();
                var company = _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
                foreach (var employee in employees)
                {
                    if (employee.EmployeeStatus == SD.Staff)
                    {
                        staff_list.Add(employee);
                    }
                }
                foreach (var staff in staff_list)
                {
                    if (User != null)
                    {
                        if (
                          staff.FirstName == User.FirstName ||
                          staff.FirstName.Contains(User.FirstName) ||
                          staff.LastName == User.LastName ||
                          staff.LastName.Contains(User.LastName) ||
                          staff.PhoneNumber == User.PhoneNumber ||
                          staff.PhoneNumber.Contains(User.PhoneNumber))
                        {

                            if (staff.AssignedTo == user.Id)
                            {
                                logged_in_supervisor_staff.Add(staff);
                            }

                        }
                    }

                    if (staff.AssignedTo == user.Id)
                    {
                        logged_in_supervisor_staff.Add(staff);
                    }
                }
                return logged_in_supervisor_staff;
            }
            throw new BusinessException("This user is not a supervisor");
        }
    }
}
