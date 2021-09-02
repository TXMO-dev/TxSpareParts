using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Utility.Interfaces;
using TxSpareParts.Core.QueryFilter;

namespace TxSpareParts.Infastructure.services
{
    public class AdminService : IAdminService
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IUnitOfWork _unitofwork;
        private readonly IEmailSender _emailsender;
        private readonly IInvoiceHandler _invoice;
        private readonly IPaymentProcessingHandler _payment;

        public AdminService(
            UserManager<ApplicationUser> usermanager,
            IUnitOfWork unitofwork,
            IEmailSender emailsender,
            IInvoiceHandler invoice,
            IPaymentProcessingHandler payment)
        {
            _usermanager = usermanager;
            _unitofwork = unitofwork;
            _emailsender = emailsender;
            _invoice = invoice;
        }

        public async Task<string> DeleteAdministratorForChief(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            var all_administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            IList<ApplicationUser> company_administrators = new List<ApplicationUser>();

            foreach (var admin in all_administrators)
            {
                if (admin.Id == company.ID)
                {
                    company_administrators.Add(admin);
                }
            }



            if (company != null)
            {
                if (user != null)
                {
                    if (all_administrators != null)
                    {
                        foreach (var administrator in all_administrators)
                        {
                            if (administrator.AdministrativeStatus != SD.ChiefAdmin &&
                                administrator.Id == user.Id
                                )
                            {
                                var result = await _usermanager.DeleteAsync(administrator);
                                if (result.Succeeded)
                                {
                                    foreach (var admin in all_administrators)
                                    {
                                        if (admin.CompanyId == administrator.CompanyId)
                                        {
                                            foreach (var employee in employees)
                                            {
                                                if (employee.CompanyId == admin.CompanyId)
                                                {
                                                    if (company_administrators.Count <= 1)
                                                    {
                                                        if (await _usermanager.GetLockoutEnabledAsync(employee))
                                                        {
                                                            await _usermanager.SetLockoutEndDateAsync(employee, DateTime.Now.AddYears(2000));
                                                            await _usermanager.SetLockoutEnabledAsync(employee, true);
                                                            await _emailsender.SendEmailAsync(employee.Email,
                                                                                              "Account Lockout",
                                                                                              "<p>Unfortunately your account has been locked out.</p>" +
                                                                                              "<p>This is because your administrator has been deleted from " +
                                                                                              "the application,</p>" +
                                                                                              $"<p>Once {company.Name} updates the app with its new administrator we will let you know</p>");
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                    }

                                }

                            }
                        }
                    }

                }
                var user_role = await _usermanager.GetRolesAsync(user);
                return $"{user.FirstName} {user.LastName} {user_role[0]} has been deleted successfully)";
            }
            throw new BusinessException("Company cannot be found for this Deletion");
        }
        public async Task<string> DeleteCustomer(string id)
        {
            var user = await _usermanager.FindByIdAsync(id);
            if (user != null)
            {
                var result = await _usermanager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    await _emailsender.SendEmailAsync(user.Email,
                        "Account Deleted",
                        "<p>Due to certain reasons or sanctions based on fraudulent activities,</p>" +
                        "<p>We have decided to delete your account, its been nice working with you, do well to email us at </p>" +
                        "<p>txwebservicesghana@gmail.com for further assistance</p>");
                    return $"Customer ({user.FirstName} {user.LastName}) has been deleted successfully";
                }
                throw new BusinessException("Customer could not be deleted successfully");
            }
            throw new BusinessException("The Customer does not exist");

        }
        public async Task<string> DeleteStaff(string id, SupervisorDTO assigntosupervisor)
        {
            var user = await _usermanager.FindByIdAsync(id);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            IList<ApplicationUser> supervisors = new List<ApplicationUser>();
            IList<ApplicationUser> staffs = new List<ApplicationUser>();
            IList<ApplicationUser> company_supervisors = new List<ApplicationUser>();
            foreach (var employee in employees)
            {
                if (employee.EmployeeStatus == SD.Supervisor)
                {
                    supervisors.Add(employee);
                }
                if (employee.EmployeeStatus == SD.Staff)
                {
                    staffs.Add(employee);
                }
            }

            foreach (var supervisor in supervisors)
            {
                if (supervisor.CompanyId == company.ID)
                {
                    company_supervisors.Add(supervisor);
                }
            }

            if (user != null)
            {
                if (user.EmployeeStatus == SD.Supervisor)
                {
                    foreach (var supervisor in supervisors)
                    {
                        foreach (var staff in staffs)
                        {
                            if (supervisor.Id == user.Id)
                            {
                                var result = await _usermanager.DeleteAsync(supervisor);
                                if (result.Succeeded)
                                {
                                    if (company_supervisors.Count <= 1)
                                    {
                                        staff.AssignedTo = null;
                                        await _usermanager.UpdateAsync(staff);
                                        await _emailsender.SendEmailAsync(staff.Email,
                                            "Supervisory Update",
                                            "The only supervisor for this company has been deactivated" +
                                            "therefore you will be temporarily managed by the company administrators");
                                    }
                                    foreach (var super in supervisors)
                                    {
                                        if (super.CompanyId == supervisor.CompanyId)
                                        {
                                            if (super.FirstName == assigntosupervisor.assignedtosupervisor ||
                                                super.FirstName.Contains(assigntosupervisor.assignedtosupervisor) ||
                                                super.LastName == assigntosupervisor.assignedtosupervisor ||
                                                super.LastName.Contains(assigntosupervisor.assignedtosupervisor))
                                            {
                                                staff.AssignedTo = super.Id;
                                                await _usermanager.UpdateAsync(staff);
                                                await _emailsender.SendEmailAsync(
                                                    staff.Email,
                                                    "Supervisor Reassigned",
                                                    $"<p>Your current supervisor {supervisor.FirstName} {supervisor.LastName} " +
                                                    $"has been deleted from the app so we are " +
                                                    $"reassigning you to a new supervisor {super.FirstName} {super.LastName}</p>");
                                            }

                                            int index = new Random().Next(company_supervisors.Count);
                                            staff.AssignedTo = company_supervisors[index].Id;
                                            await _usermanager.UpdateAsync(staff);
                                            await _emailsender.SendEmailAsync(
                                                    staff.Email,
                                                    "Supervisor Reassigned",
                                                    $"<p>Your current supervisor {supervisor.FirstName} {supervisor.LastName} " +
                                                    $"has been deleted from the app so we are " +
                                                    $"reassigning you to a new supervisor " +
                                                    $"{company_supervisors[index].FirstName} {company_supervisors[index].LastName}</p>");

                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (user.EmployeeStatus == SD.Staff)
                {
                    foreach (var staff in staffs)
                    {
                        if (staff.Id == user.Id)
                        {
                            var result = await _usermanager.DeleteAsync(staff);
                            if (result.Succeeded)
                            {
                                await _emailsender.SendEmailAsync(staff.Email,
                                                                  "Account Deletion",
                                                                  $"Your account has been deleted from the platform " +
                                                                  $"for reasons only known by the company {company.Name}");
                            }
                        }
                    }
                }
            }
            throw new BusinessException("The user cannot be found...");
        }
        public async Task<string> LockoutServiceForAdmin(string id, SupervisorDTO assigntosupervisor)
        {
            var user = await _usermanager.FindByIdAsync(id);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            var company = new Company();
            IList<ApplicationUser> staff_list = new List<ApplicationUser>();
            IList<ApplicationUser> supervisors = new List<ApplicationUser>(); ;
            IList<ApplicationUser> supervisor_for_company = new List<ApplicationUser>(); ;
            IList<string> old_assigned_to_list = new List<string>();   
            IList<ApplicationUser> old_staff_list = new List<ApplicationUser>();
            foreach (var employee in employees)
            {
                {
                    if (employee.EmployeeStatus == SD.Staff)
                    {
                        staff_list.Add(employee);
                    }

                    if (user != null)
                    {
                        company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
                        if (employee.EmployeeStatus == SD.Supervisor)
                        {
                            if (employee.CompanyId == company.ID)
                            {
                                supervisors.Add(employee);
                            }
                        }
                    }
                }
            }

            if (await _usermanager.IsInRoleAsync(user, SD.Employee) ||
                await _usermanager.IsInRoleAsync(user, SD.Customer)
               )
            {
                if (user.LockoutEnd > DateTime.Now)
                {

                    await _usermanager.SetLockoutEndDateAsync(user, DateTime.Now);
                    await _usermanager.SetLockoutEnabledAsync(user, false);
                    foreach (var old_id in old_assigned_to_list)
                    {
                        var old_user = await _usermanager.FindByIdAsync(old_id);
                        if (old_user != null)
                        {
                            if (user.EmployeeStatus == SD.Supervisor && old_user.Id == user.Id)
                            {
                                foreach (var old_staff in old_staff_list)
                                {
                                    if (old_staff.AssignedTo == old_user.Id)
                                    {
                                        await _usermanager.UpdateAsync(old_staff);

                                        await _emailsender.SendEmailAsync(
                                            old_staff.Email,
                                            "Reassign Update",
                                            $"<p>Your former supervisor {old_user.FirstName} {old_user.LastName} ({SD.Supervisor}), </p>" +
                                            "<p>has been unlocked and you have been reassigned back to your former supervisor. </p>" +
                                            $"<p>Please do well to welcome him back on {old_user.Email}</p>");

                                        await _emailsender.SendEmailAsync(
                                            old_user.Email,
                                            "Account unlocked",
                                            $"<p>Hello {old_user.FirstName} {old_user.LastName} ({SD.Supervisor}), your account has been unlocked</p>" +
                                            $"<p>and all your staff have been reassigned back to you</p>");
                                    }
                                }
                            }
                        }
                    }
                    return $"Unlock for {user.FirstName} {user.LastName} was successful";
                }
                if (user.LockoutEnd == null || user.LockoutEnd == DateTime.Now)
                {
                    await _usermanager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(2000));
                    await _usermanager.SetLockoutEnabledAsync(user, true);
                    if (user.EmployeeStatus == SD.Supervisor)
                    {
                        foreach (var staff in staff_list)
                        {
                            foreach (var supervisor in supervisors)
                            {
                                if (staff.AssignedTo == user.Id)
                                {
                                    if (supervisor.CompanyId == company.ID)
                                    {
                                        supervisor_for_company.Add(supervisor);
                                        if (assigntosupervisor.assignedtosupervisor != null)
                                        {
                                            if (
                                            supervisor.FirstName == assigntosupervisor.assignedtosupervisor ||
                                            supervisor.FirstName.Contains(assigntosupervisor.assignedtosupervisor) ||
                                            supervisor.LastName == assigntosupervisor.assignedtosupervisor ||
                                            supervisor.LastName.Contains(assigntosupervisor.assignedtosupervisor))
                                            {
                                                staff.AssignedTo = supervisor.Id;
                                                await _usermanager.UpdateAsync(staff);
                                                await _emailsender.SendEmailAsync(
                                                    staff.Email,
                                                    "Supervisor Reassigned",
                                                    "<p>your current supervisors account has been locked.</p>" +
                                                    "<p> With that being said we have reassigned you to a new supervisor:" +
                                                    $" {supervisor.FirstName} {supervisor.LastName} ({SD.Supervisor}) of {company.Name}" +
                                                    $" You can email him on {supervisor.Email}" +
                                                    "Please do well to introduce yourself to him, thank you");
                                            }

                                        }

                                        int index = new Random().Next(supervisor_for_company.Count());
                                        staff.AssignedTo = supervisor_for_company[index].Id;
                                        await _usermanager.UpdateAsync(staff);
                                        await _emailsender.SendEmailAsync(
                                                staff.Email,
                                                "Supervisor Reassigned",
                                                "<p>your current supervisors account has been locked</p>" +
                                                "<p> With that being said we have reassigned you to a new supervisor:" +
                                                $" {supervisor_for_company[index].FirstName} {supervisor_for_company[index].LastName} ({SD.Supervisor}) " +
                                                $"of {company.Name}" +
                                                $"Please do well to introduce yourself to him via {supervisor_for_company[index].Email}, thank you");

                                    }
                                }
                            }
                        }
                    }
                    var user_role = await _usermanager.GetRolesAsync(user);
                    return $"Lock for {user.FirstName} {user.LastName} {user_role[user_role.Count]} was successful";
                }

            }
            throw new BusinessException("Lock/Unlock operation was unsuccessful.");

        }
        public async Task<string> LockoutServiceForChief(string id, SupervisorDTO assigntosupervisor)
        {
            var user = await _usermanager.FindByIdAsync(id);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            var company = new Company();
            IList<ApplicationUser> staff_list = new List<ApplicationUser>(); ;
            IList<ApplicationUser> supervisors = new List<ApplicationUser>(); ;
            IList<ApplicationUser> supervisor_for_company = new List<ApplicationUser>(); ;
            IList<string> old_assigned_to_list = new List<string>();
            IList<ApplicationUser> old_staff_list = new List<ApplicationUser>(); ;
            foreach (var employee in employees)
            {
                {
                    if (employee.EmployeeStatus == SD.Staff)
                    {
                        staff_list.Add(employee);
                    }

                    if (user != null)
                    {
                        company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
                        if (employee.EmployeeStatus == SD.Supervisor)
                        {
                            if (employee.CompanyId == company.ID)
                            {
                                supervisors.Add(employee);
                            }
                        }
                    }
                }

                if (
                (user.AdministrativeStatus != SD.ChiefAdmin && await _usermanager.IsInRoleAsync(user, SD.Admin)) ||
                await _usermanager.IsInRoleAsync(user, SD.Employee) ||
                await _usermanager.IsInRoleAsync(user, SD.Customer)
                )
                {
                    if (user.LockoutEnd > DateTime.Now)
                    {

                        await _usermanager.SetLockoutEndDateAsync(user, DateTime.Now);
                        await _usermanager.SetLockoutEnabledAsync(user, false);
                        foreach (var old_id in old_assigned_to_list)
                        {
                            var old_user = await _usermanager.FindByIdAsync(old_id);
                            if (old_user != null)
                            {
                                if (user.EmployeeStatus == SD.Supervisor && old_user.Id == user.Id)
                                {
                                    foreach (var old_staff in old_staff_list)
                                    {
                                        if (old_staff.AssignedTo == old_user.Id)
                                        {
                                            await _usermanager.UpdateAsync(old_staff);

                                            await _emailsender.SendEmailAsync(
                                                old_staff.Email,
                                                "Reassign Update",
                                                $"<p>Your former supervisor {old_user.FirstName} {old_user.LastName} ({SD.Supervisor}), </p>" +
                                                "<p>has been unlocked and you have been reassigned back to your former supervisor. </p>" +
                                                $"<p>Please do well to contact him on {old_user.Email}</p>");

                                            await _emailsender.SendEmailAsync(
                                                old_user.Email,
                                                "Account unlocked",
                                                $"<p>Hello {old_user.FirstName} {old_user.LastName} ({SD.Supervisor}), your account has been unlocked</p>" +
                                                $"<p>and all your staff have been reassigned back to you</p>");
                                        }
                                    }
                                }
                            }
                        }
                        return $"Unlock for {user.FirstName} {user.LastName} was successful";
                    }
                    if (user.LockoutEnd == null || user.LockoutEnd == DateTime.Now)
                    {
                        await _usermanager.SetLockoutEndDateAsync(user, DateTime.Now.AddYears(2000));
                        await _usermanager.SetLockoutEnabledAsync(user, true);
                        if (user.EmployeeStatus == SD.Supervisor)
                        {
                            foreach (var staff in staff_list)
                            {
                                foreach (var supervisor in supervisors)
                                {
                                    if (staff.AssignedTo == user.Id)
                                    {
                                        if (supervisor.CompanyId == company.ID)
                                        {
                                            supervisor_for_company.Add(supervisor);
                                            if (assigntosupervisor.assignedtosupervisor != null)
                                            {
                                                if (
                                                supervisor.FirstName == assigntosupervisor.assignedtosupervisor ||
                                                supervisor.FirstName.Contains(assigntosupervisor.assignedtosupervisor) ||
                                                supervisor.LastName == assigntosupervisor.assignedtosupervisor ||
                                                supervisor.LastName.Contains(assigntosupervisor.assignedtosupervisor))
                                                {
                                                    var old_assigned_to = staff.AssignedTo;
                                                    var old_user = await _usermanager.FindByIdAsync(old_assigned_to);
                                                    old_assigned_to_list.Add(old_assigned_to);
                                                    old_staff_list.Add(staff);
                                                    staff.AssignedTo = supervisor.Id;
                                                    await _usermanager.UpdateAsync(staff);

                                                    await _emailsender.SendEmailAsync(
                                                        staff.Email,
                                                        "Supervisor Reassigned",
                                                        $"<p>your current supervisor {old_user.FirstName} {old_user.LastName}'s" +
                                                        $" account has been locked.</p>" +
                                                        "<p> With that being said we have reassigned you to a new supervisor:" +
                                                        $" {supervisor.FirstName} {supervisor.LastName} ({SD.Supervisor}) of {company.Name}" +
                                                        $" You can email him on {supervisor.Email}" +
                                                        "Please do well to introduce yourself to him as well, thank you");

                                                    await _emailsender.SendEmailAsync(
                                                        old_user.Email,
                                                        "Account unlocked",
                                                        $"<p>Hello {old_user.FirstName} {old_user.LastName} ({SD.Supervisor}), your account has been locked</p>" +
                                                        $"<p>and all your staff have been reassigned to another supervisor.</p>");
                                                }
                                            }

                                            int index = new Random().Next(supervisor_for_company.Count());
                                            var assigned_to = staff.AssignedTo;
                                            var old_assigned_user = await _usermanager.FindByIdAsync(assigned_to);
                                            old_assigned_to_list.Add(assigned_to);
                                            old_staff_list.Add(staff);
                                            staff.AssignedTo = supervisor_for_company[index].Id;
                                            await _usermanager.UpdateAsync(staff);
                                            await _emailsender.SendEmailAsync(
                                                    staff.Email,
                                                    "Supervisor Reassigned",
                                                    $"<p>your current supervisor {old_assigned_user.FirstName} {old_assigned_user.LastName}'s " +
                                                    $"account has been locked</p>" +
                                                    "<p> With that being said we have reassigned you to a new supervisor:" +
                                                    $" {supervisor_for_company[index].FirstName} {supervisor_for_company[index].LastName} ({SD.Supervisor}) " +
                                                    $"of {company.Name}" +
                                                    $"Please do well to introduce yourself to him via {supervisor_for_company[index].Email}, thank you");

                                            await _emailsender.SendEmailAsync(
                                                       old_assigned_user.Email,
                                                       "Account unlocked",
                                                       $"<p>Hello {old_assigned_user.FirstName} {old_assigned_user.LastName} ({SD.Supervisor}), your account has been locked</p>" +
                                                       $"<p>and all your staff have been reassigned to another supervisor.</p>");

                                        }
                                    }
                                }
                            }
                        }
                        var user_role = await _usermanager.GetRolesAsync(user);
                        return $"Lock for {user.FirstName} {user.LastName} ( {user_role[user_role.Count]} ) was successful";
                    }
                }
                throw new BusinessException("Lock system is not working");
            }
            throw new BusinessException("The user does not exist");
        }
        public async Task<string> UpdateService(string id, UpdateUserDTO user, string[] roles)
        {
            var found_user = await _usermanager.FindByIdAsync(id);
            if (found_user != null)
            {
                foreach (var role in roles)
                {
                    if (await _usermanager.IsInRoleAsync(found_user, role))
                    {
                        if (role == SD.Admin)
                        {
                            if (user.Email != null)
                            {
                                found_user.Email = user.Email;
                            }
                            if (user.FirstName != null)
                            {
                                found_user.FirstName = user.FirstName;
                            }
                            if (user.LastName != null)
                            {
                                found_user.LastName = user.LastName;
                            }
                            if (user.PhoneNumber != null)
                            {
                                found_user.PhoneNumber = user.PhoneNumber;
                            }
                        }
                        if (role == SD.Employee)
                        {
                            if (user.EmployeeStatus == SD.Staff)
                            {
                                if (user.Email != null)
                                {
                                    found_user.Email = user.Email;
                                }
                                if (user.FirstName != null)
                                {
                                    found_user.FirstName = user.FirstName;
                                }

                                if (user.LastName != null)
                                {
                                    found_user.LastName = user.LastName;
                                }
                                if (user.EmployeeStatus == SD.Supervisor)
                                {
                                    found_user.EmployeeStatus = SD.Staff;
                                    await _emailsender.SendEmailAsync(found_user.Email, "Employee Status Changed", "On reasons or sanctions been agreed upon, " +
                                        $"we finally came to a conclusion to change your job status from {SD.Supervisor} to {SD.Staff}");
                                }
                                if (user.EmployeeStatus == SD.Staff)
                                {
                                    found_user.EmployeeStatus = SD.Supervisor;
                                    await _emailsender.SendEmailAsync(found_user.Email, "Employee Status Changed", "On reasons or sanctions been agreed upon, " +
                                        $"we finally came to a conclusion to change your job status from {SD.Staff} to {SD.Supervisor}");
                                }
                                if (user.PhoneNumber != null)
                                {
                                    found_user.PhoneNumber = user.PhoneNumber;
                                }
                            }
                        }
                        if (role == SD.Customer)
                        {
                            if (user.Email != null)
                            {
                                found_user.Email = user.Email;
                            }
                            if (user.FirstName != null)
                            {
                                found_user.FirstName = user.FirstName;
                            }

                            if (user.LastName != null)
                            {
                                found_user.LastName = user.LastName;
                            }
                            if (user.PhoneNumber != null)
                            {
                                found_user.PhoneNumber = user.PhoneNumber;
                            }
                            if (user.PhysicalAddress != null)
                            {
                                found_user.PhysicalAdress = user.PhysicalAddress;
                            }
                            if (user.DigitalAddress != null)
                            {
                                found_user.DigitalAddress = user.DigitalAddress;
                            }
                            if (user.City != null)
                            {
                                found_user.City = user.City;
                            }
                            if (user.Region != null)
                            {
                                found_user.Region = user.Region;
                            }
                        }
                    }
                }
                return $"{found_user.FirstName} {found_user.LastName} has been updated successfully";
            }
            throw new BusinessException("User does not exist");
        }
        public async Task<PagedList<Order>> GetAllChiefOrdersService(StarredCompaniesQueryFIlter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.userId);
            filter.pagination.pageNumber = filter.pagination.pageNumber == 0 ? 1 : filter.pagination.pageNumber;
            filter.pagination.pageSize = filter.pagination.pageSize == 0 ? 10 : filter.pagination.pageSize;
            if (user != null && user.AdministrativeStatus == SD.ChiefAdmin)
            {

                var all_assigned_orders = await _unitofwork.OrderRepository.GetAll(e => e.Assignedto == user.Id);
                return PagedList<Order>.Create(all_assigned_orders.ToArray(),(int)filter.pagination.pageNumber,(int)filter.pagination.pageSize);
            }
            throw new BusinessException("The user either does not exist or is not a chief administrator");
        }
        public async Task<string> UpdateChiefOrderService(string userId, string orderId, Order order)
        {
            var chief_admin = await _usermanager.FindByIdAsync(userId);
            if (chief_admin != null && chief_admin.AdministrativeStatus == SD.ChiefAdmin)
            {
                var order_details = await _unitofwork.OrderRepository.Get(orderId);

                if (order_details != null)
                {
                    var customer = await _usermanager.FindByIdAsync(order_details.UserID);
                    order_details = order;
                    _unitofwork.OrderRepository.Update(order_details);
                    await _unitofwork.Save();
                    if (
                        order_details.TrackingNumber != null &&
                        order_details.Carrier != null &&
                        order_details.EstimatedDeliveryDate != null &&
                        order_details.OrderStatus == SD.SHI)
                    {

                        var generated_receipt = await _invoice.GenerateReceipt(customer.Id, order_details.RefferenceCode);
                        await _emailsender.SendEmailAsync(
                            customer.Email,
                            "Order Shipped",
                            $"<p> Congratulations {customer.FirstName} {customer.LastName}, your order has been shipped</p>" +
                            $"<p> You can view or download your receipt here, <a href = {generated_receipt}>Receipt</a></p>");
                        await _emailsender.SendEmailAsync(
                            "txwebservice@gmail.com",
                            "Order Shipped",
                            $"<p> This is to notify you of an order shipped with refference Code {order_details.RefferenceCode}" +
                            $"The customers name is {customer.FirstName} {customer.LastName}</p>" +
                            $"<p> You can view or download the receipt here, <a href = {generated_receipt}>Receipt</a></p>");
                    }
                    if (order_details.OrderStatus == SD.CAN)
                    {
                        var generated_receipt = await _invoice.GenerateReceipt(customer.Id, order_details.RefferenceCode);
                        await _emailsender.SendEmailAsync(
                            customer.Email,
                            "Order Cancelled",
                            $"We are sorry but your order has been cancelled, we deeply regret our order" +
                            $"<p> You can view or download your receipt here, <a href = {generated_receipt}>Receipt</a></p>");
                        await _emailsender.SendEmailAsync(
                            "txwebservice@gmail.com",
                             "Order Cancelled",
                            $"<p> An order has been cancelled, </p>" +
                            $"<p> Refference Code: {order_details.RefferenceCode} </p>" +                             
                            $"<p> You can view or download your receipt here, <a href = {generated_receipt}>Receipt</a></p>");
                    }
                    return "Order has been updated successfully";
                }
                throw new BusinessException("The order does not exist");
            }
            throw new BusinessException("The user either does not exist or is not a Chief Administrator");
        }
        public async Task<string> DeleteChiefOrderService(string userId, string orderId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && user.AdministrativeStatus == SD.ChiefAdmin)
            {
                var order = await _unitofwork.OrderRepository.Get(orderId);
                if (order != null)
                {
                    var customer = await _usermanager.FindByIdAsync(order.UserID);
                    if (customer != null)
                    {
                        _unitofwork.OrderRepository.Remove(order.Id);
                        await _unitofwork.Save();
                        await _emailsender.SendEmailAsync(
                            customer.Email,
                            "Your Order Has been Deleted",
                            "<p>We have deleted your order due to a verification process made by our team," +
                            " we deeply apologize but do log in to make another purchase</p>");

                    }
                    throw new BusinessException("The customer does not exist");
                }
                throw new BusinessException("The order does not exist");
            }
            throw new BusinessException("THe user either does not exist or is not a chief administrator");

        }   
        public async Task<string> HandleDisputeChiefService(string userId, string from, string to)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && user.AdministrativeStatus == SD.ChiefAdmin)
            {
                var result = await _payment.HandleDisputes(from, to);
                return result;
            }
            throw new BusinessException("Disputes were not able to be run");
        }  
    }
}
