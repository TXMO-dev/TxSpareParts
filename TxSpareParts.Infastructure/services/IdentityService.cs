using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Infastructure.Repository;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Infastructure.services
{
    public class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly RoleManager<IdentityRole> _rolemanager;
        private readonly IUnitOfWork _unitofwork;
        private readonly IEmailSender _emailsender;
        private readonly IJwtTokenHandler _jwttoken;
        private readonly IEmailConfirmationHandler _emailconfirm;
        private readonly IConfiguration _configuration;
        private readonly IPhoneNumberVerification _phonenum;

        public IdentityService(
            UserManager<ApplicationUser> usermanager,
            RoleManager<IdentityRole> rolemanager,
            IEmailSender emailsender,
            IEmailConfirmationHandler emailconfirm,
            IJwtTokenHandler jwttoken,
            IPhoneNumberVerification phonenum,
            IConfiguration configuration,
            IUnitOfWork unitofwork)
        {
            _usermanager = usermanager;
            _rolemanager = rolemanager;
            _jwttoken = jwttoken;
            _emailsender = emailsender;
            _emailconfirm = emailconfirm;
            _configuration = configuration;
            _phonenum = phonenum;
            _unitofwork = unitofwork;
        }
           
        public async Task<ApplicationUser> CheckForUserById(string id)
        {
            
            var result = await _usermanager.FindByIdAsync(id);
            if (result != null)
                return result;
            return null;  
        }

        public async Task<string> ConfirmEmailService(EmailConfirmationQueryFilter filter)
        {
            if(filter.userId != null)
            {
                var user = await _usermanager.FindByIdAsync(filter.userId);
                if(user == null)
                {
                    throw new BusinessException("User does not exist");
                }
                if(filter.token != null)
                {
                    var normaltoken = WebEncoders.Base64UrlDecode(filter.token);
                    var string_token = Encoding.UTF8.GetString(normaltoken);
                    var result = await _usermanager.ConfirmEmailAsync(user, string_token);
                    if (result.Succeeded)
                        return $"Congratulations {user.FirstName} {user.LastName}, your email has been successfully confirmed";

                    throw new BusinessException("Token does not exist");
                }   
            }
            throw new BusinessException("userId is empty");   
        }

        public async Task<string> ForgotPasswordService(ApplicationUser User)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if(user != null)
            {
                var reset_token = await _usermanager.GeneratePasswordResetTokenAsync(user);
                var token_bytes = Encoding.UTF8.GetBytes(reset_token);
                var encoded_token = WebEncoders.Base64UrlEncode(token_bytes);
                string url = $"{_configuration["AppUrl"]}/ResetPassword?email={user.Email}&token={encoded_token}";
                await _emailsender.SendEmailAsync(user.Email, "Reset Password Request", 
                                                              "<p>Kindly click on the link " +
                                                              $"<a href = {url}>here</a>  " +
                                                              $"or copy link below </n> {url}</p>");     
                return $"Email has been sent to {user.Email} to reset password";
            }
            throw new BusinessException("The user associated with this email does not exist");  
        }

        public async Task<ApplicationUser> GetMeProfileService(UpdateUserQueryFilter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.Id);
            if (user != null)
                return user;
            return user;
        }

        public async Task<string> LoginSocial(string email)
        {
            var user = await _usermanager.FindByEmailAsync(email);
            if (user != null)
                return await _jwttoken.GenerateToken(user);
            throw new BusinessException("This user does not exist");
        }

        public async Task<string> LoginUserService(ApplicationUser User)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if(user != null)
            {
                var check_password = await _usermanager.CheckPasswordAsync(user, User.Password);
                if(check_password)   
                    return await _jwttoken.GenerateToken(user);   
                
                throw new BusinessException("Invalid Password");
            }
            throw new BusinessException("Invalid User");
        }

        public async Task<string> RegisterAdminService(ApplicationUser User)
        {
            IList<ApplicationUser> administrators_of_company = new List<ApplicationUser>();
            var the_company = await _unitofwork.CompanyRepository.Get(User.CompanyId);
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if (user != null && 
                user.PasswordHash == null && 
                user.FirstName == null && 
                user.LastName == null &&
                user.PhoneNumber == null)
            {
                user.FirstName = User.FirstName;
                user.LastName = User.LastName;
                user.PhoneNumber = User.PhoneNumber;
                user.EmailConfirmed = true;
                user.isVerified = true;
               var pass_res = await _usermanager.AddPasswordAsync(user, User.Password);

                if (!pass_res.Succeeded)
                    throw new BusinessException("Password could not create successfully");

                var result = await _usermanager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var nuser = await _usermanager.FindByEmailAsync(User.Email);
                    var all_user_images = await _unitofwork.UserImageRepository.GetAll(e => e.UserId == nuser.Id);
                    IList<UserImage> user_list = new List<UserImage>();   
                    foreach (var all_u in all_user_images)
                    {
                        user_list.Add(all_u);
                    }
                    if (user_list.Count == 0)
                    {
                        await _unitofwork.UserImageRepository.Add(new UserImage
                        {
                            UserId = user.Id,
                            ImageUrl = "<DEFAULT IMAGE FROM FIREBASE"
                        });
                    }
                    if (!await _rolemanager.RoleExistsAsync(SD.Admin))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Admin));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Customer))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Customer));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Employee))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Employee));
                    }
                    await _usermanager.AddToRoleAsync(user, SD.Admin);
                    if(user.PhoneNumber != null)
                    {
                        await _phonenum.SendToken(user.PhoneNumber);
                    }
                    if(await _usermanager.IsInRoleAsync(user, SD.Admin))
                    {
                        var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
                        var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
                        var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
                        foreach(var administrator in administrators)
                        {
                            if(administrator.CompanyId == user.CompanyId)
                            {
                                administrators_of_company.Add(administrator);
                            }
                        }
                        foreach(var administrator in administrators)
                        {
                            foreach(var employee in employees)
                            {
                                if((administrator.CompanyId == user.CompanyId)&& (employee.CompanyId == user.CompanyId))
                                {
                                    if(administrators_of_company.Count == 1)
                                    {
                                        if(!await _usermanager.GetLockoutEnabledAsync(employee)) 
                                        {
                                            await _usermanager.SetLockoutEndDateAsync(employee, DateTime.Now);
                                            await _usermanager.SetLockoutEnabledAsync(employee, false);
                                            await _emailsender.SendEmailAsync(
                                                employee.Email, 
                                                "Accounts Unlocked",   
                                                $"<p>Your company, {company.Name} has just gotten " +
                                                $"a new {SD.Admin}, threfore your account has been unlocked.</p>");
                                        }    
                                    }
                                }
                            }
                        }
                    }
                    return  await _jwttoken.GenerateToken(user);  
                }
                throw new BusinessException("Administrator failed to create successfully");
            }
            throw new BusinessException($"This user is not assigned for the position of an administrator at {the_company.Name}");
        }

        public async Task<string> RegisterEmploymentService(ApplicationUser User)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            IList<ApplicationUser> staffs = new List<ApplicationUser>();
            foreach(var employee in employees)
            {
                if(employee.EmployeeStatus == SD.Staff)
                {
                    staffs.Add(employee);
                }

            }
            if (company == null)
                throw new BusinessException("Company does not exist in our database");

            if (user != null)
            {
                user.FirstName = User.FirstName;
                user.LastName = User.LastName;
                user.PhoneNumber = User.PhoneNumber;
                user.EmailConfirmed = true;
                user.isVerified = true;
                var pass_res = await _usermanager.AddPasswordAsync(user, User.Password);

                if (!pass_res.Succeeded)
                    throw new BusinessException("Password failed during creation");


                var result = await _usermanager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    var nuser = await _usermanager.FindByEmailAsync(User.Email);
                    var all_user_images = await _unitofwork.UserImageRepository.GetAll(e => e.UserId == nuser.Id);
                    IList<UserImage> user_list = new List<UserImage>();
                    foreach (var all_u in all_user_images)
                    {
                        user_list.Add(all_u);
                    }
                    if (user_list.Count == 0)
                    {
                        await _unitofwork.UserImageRepository.Add(new UserImage
                        {
                            UserId = user.Id,
                            ImageUrl = "<DEFAULT IMAGE FROM FIREBASE>"
                        });
                    }
                    if (!await _rolemanager.RoleExistsAsync(SD.Admin))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Admin));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Customer))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Customer));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Employee))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Employee));
                    }
                    await _usermanager.AddToRoleAsync(user, SD.Employee);
                    if (user.PhoneNumber != null)
                    {
                        await _phonenum.SendToken(user.PhoneNumber);
                    }
                    if(user.EmployeeStatus == SD.Supervisor)
                    {
                        foreach(var staff in staffs)
                        {
                            if(user.CompanyId == staff.CompanyId && staff.AssignedTo == null)
                            {
                                staff.AssignedTo = user.Id;
                                await _usermanager.UpdateAsync(staff);
                                await _emailsender.SendEmailAsync(staff.Email, 
                                                                  "Supervisory Update", 
                                                                  $"<p>{company.Name} has updated its Supervisor, your new supervisor is</p>" +
                                                                  $"{user.FirstName} {user.LastName} please do well to contact him/her on {user.Email}");
                            }
                        }
                    }
                    return await _jwttoken.GenerateToken(user);
                }
                throw new BusinessException($"Employee {user.EmployeeStatus} failed to create successfully for {company.Name}");
            }
            throw new BusinessException($"This user is not assigned for the position of a {user.EmployeeStatus} at {company.Name}.");   
        }

        public async Task<string> RegisterUserService(ApplicationUser User,string role)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if (user == null)
            {
                    User.UserName = User.Email;
                    var results = await _usermanager.CreateAsync(User, User.Password);
                    if (results.Succeeded)
                    {
                    var nuser = await _usermanager.FindByEmailAsync(User.Email);
                    var all_user_images = await _unitofwork.UserImageRepository.GetAll(e => e.UserId == nuser.Id);

                    var registered_user = await _usermanager.FindByEmailAsync(User.Email);
                    if (all_user_images.ToList().Count() == 0)  
                    {
                        await _unitofwork.UserImageRepository.Add(new UserImage
                        {
                            UserId = registered_user.Id,
                            ImageUrl = "<DEFAULT IMAGE FROM FIREBASE"
                        });
                    }
                    if (!await _rolemanager.RoleExistsAsync(SD.Admin))
                        {
                            await _rolemanager.CreateAsync(new IdentityRole(SD.Admin));
                        }

                        if (!await _rolemanager.RoleExistsAsync(SD.Customer))
                        {
                            await _rolemanager.CreateAsync(new IdentityRole(SD.Customer));
                        }

                        if (!await _rolemanager.RoleExistsAsync(SD.Employee))
                        {   
                            await _rolemanager.CreateAsync(new IdentityRole(SD.Employee));
                        }
                        await _usermanager.AddToRoleAsync(User, role);
                    //create email confirmation token async
                    var url = await _emailconfirm.GenerateToken(User);
                    await _emailsender.SendEmailAsync(
                        User.Email, 
                        "Registration Successful", 
                        $"Please click on the link to <a href={url}>confirm</a> your email address" +
                        $"or copy link below: </n> {url}");
                    if (User.PhoneNumber != null)
                    {
                        await _phonenum.SendToken(User.PhoneNumber);
                    }
                        return await _jwttoken.GenerateToken(User);
                    }
               //// Code for debugging.
               //var message = string.Join(", ", results.Errors.Select(x => "Code " + x.Code + " Description " + x.Description));
               // return $"{message}";

            }
            throw new BusinessException("User already exists");
        }

        public async Task<string> RegisterUserSocialService(ApplicationUser User)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if (user == null)
            {
                var results = await _usermanager.CreateAsync(User);
                if (results.Succeeded)
                {
                    var nuser = await _usermanager.FindByEmailAsync(User.Email);
                    var all_user_images = await _unitofwork.UserImageRepository.GetAll(e => e.UserId == nuser.Id);
                    IList<UserImage> user_list = new List<UserImage>();
                    foreach (var all_u in all_user_images)
                    {
                        user_list.Add(all_u);
                    }
                    if (user_list.Count == 0)
                    {
                        await _unitofwork.UserImageRepository.Add(new UserImage
                        {
                            UserId = user.Id,
                            ImageUrl = "<DEFAULT IMAGE FROM FIREBASE"
                        });
                    }
                    if (!await _rolemanager.RoleExistsAsync(SD.Admin))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Admin));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Customer))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Customer));
                    }

                    if (!await _rolemanager.RoleExistsAsync(SD.Employee))
                    {
                        await _rolemanager.CreateAsync(new IdentityRole(SD.Employee));
                    }
                    await _usermanager.AddToRoleAsync(User, SD.Customer);
                    
                    await _emailsender.SendEmailAsync(
                        User.Email,
                        "Registration Successful",
                        "Thank you for registering for Tx Spare Parts" +
                        "Please do not forget to update your settings in your profile. Enjoy the experience."
                        );
                    //if (User.PhoneNumber != null)
                    //{
                    //    await _phonenum.SendToken(User.PhoneNumber);
                    //}
                    return await _jwttoken.GenerateToken(User);
                }
                // Code for debugging.
                //var message = string.Join(", ", results.Errors.Select(x => "Code " + x.Code + " Description " + x.Description));
                //return $"{message}";          

            }
            throw new BusinessException("User could not be created.");
        }

        public async Task<string> ResetPasswordService(ResetPasswordDTO reset_password)
        {
            if (reset_password.Email != null)
            {
                var user = await _usermanager.FindByEmailAsync(reset_password.Email);
                if (user == null)
                    throw new BusinessException("The user with this email does not exist");  
                if (reset_password.Token != null)
                {
                    var decoded_token = WebEncoders.Base64UrlDecode(reset_password.Token);     
                    var normal_token = Encoding.UTF8.GetString(decoded_token);

                    if (reset_password.NewPassword != reset_password.ConfirmNewPassword)
                        return "New Passwords do not match";

                    var result = await _usermanager.ResetPasswordAsync(
                        user,
                        normal_token,
                        reset_password.NewPassword);       

                    if (result.Succeeded)
                        return "Password has been reset successfully";

                    throw new BusinessException("Password could not reset successfully");
                }
                throw new BusinessException("token is either null or invalid");
            }
            throw new BusinessException("email is either null or invalid");
        }

        public async Task<string> SendAdminEmploymentEmail(ApplicationUser User, string companyname)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);

            var the_company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.Name == companyname || e.Name.Contains(companyname));
            if (user == null)
            {
                User.UserName = User.Email;
                User.CompanyId = the_company.ID;
                var result = await _usermanager.CreateAsync(User);
                if (result.Succeeded)
                {
                    string url = $"{_configuration["AppUrl"]}/RegisterAdmin?email={User.Email}";

                    await _emailsender.SendEmailAsync(
                        User.Email,
                        "Congratulations on Your Employment",
                        $"<h1>Congratulations on your Employment at {the_company.Name} as an {SD.Admin}</h1>" +
                        $"So basically your role as an {SD.Admin} is: </n>" +
                        "<ol>" +
                        "<li>Creation and management of companies retailing on the app.</li>" +
                        "<li>Manages all users in the application</li>" +
                        "<li>Creates Employees with status of both Supervisor and Staff for a company but mostly supervisor.</li>" +
                        "<li>Assists in the management of products and orders when the need arises</li>" +
                        "e.t.c" +
                        "</ol>" +
                        $"Kindly click on the link <a href = {url}> here </a> to register.");
                    return $"Admin Registration to {User.Email} sent successfully";
                }
                
            }
            throw new BusinessException("The email was not able to send successfully");
            
          
        }

        public async Task<string> SendEmploymentEmail(ApplicationUser User, string companyname, string employeestatus)  
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.Name == companyname || e.Name.Contains(companyname));
            if (user == null && company != null)
            {
                User.UserName = User.Email;
                User.CompanyId = company.ID;
                User.EmployeeStatus = employeestatus;
                IList<ApplicationUser> supervisor_list = new List<ApplicationUser>();
                var supervisors = await _usermanager.GetUsersInRoleAsync(SD.Employee);
                if (User.EmployeeStatus == SD.Staff)
                {
                    foreach(var supervisor in supervisors)
                    {
                        if(supervisor.EmployeeStatus == SD.Supervisor)
                        {
                            supervisor_list.Add(supervisor);
                        }

                    } 
                }
                
                var result = await _usermanager.CreateAsync(User);
                if (User.SupervisorName != null && User.EmployeeStatus == SD.Staff)
                {
                    foreach (var supervisor in supervisor_list)
                    {

                        if (supervisor.FirstName == User.SupervisorName ||
                            supervisor.FirstName.Contains(User.SupervisorName) ||
                            supervisor.LastName == User.SupervisorName ||
                            supervisor.LastName.Contains(User.SupervisorName))
                        {
                            User.AssignedTo = supervisor.Id;
                            await _usermanager.UpdateAsync(User);
                        }
                    }
                }
                if(User.EmployeeStatus == SD.Staff)
                {
                    int index = new Random().Next(supervisor_list.Count());
                    if (supervisor_list[index].EmployeeStatus == SD.Supervisor)
                    {
                        User.AssignedTo = supervisor_list[index].Id;
                        await _usermanager.UpdateAsync(User);
                    }         
                }
                
                if (result.Succeeded)
                {
                    string url = $"{_configuration["AppUrl"]}/RegisterEmployee?email={User.Email}&employeestatus={SD.Supervisor}";    

                    if(User.EmployeeStatus == SD.Staff)
                    {
                        url = $"{_configuration["AppUrl"]}/RegisterEmployee?email={User.Email}&employeestatus={SD.Staff}";  
                        var staff_supervisor = await _usermanager.FindByIdAsync(User.AssignedTo);
                        if (staff_supervisor == null)
                            throw new BusinessException("This supervisor does not exist");
                        await _emailsender.SendEmailAsync(
                        User.Email,
                        "Congratulations on Joining the TX Family",   
                        $"<h1>Welcome to TX Spare Parts, as a {User.EmployeeStatus} from {company.Name} " +
                        $"So basically your role as a {User.EmployeeStatus} is: </n>" +
                        "<ol>" +
                        "<li>Updating the product catalog of the company.</li>" +
                        "<li>Managing and processing of product orders.</li>" +
                        "<li>Customer Support (via Call and Online) done shiftly.</li>" +
                        "e.t.c" +
                        $"Your supervisor's name will be {staff_supervisor.FirstName} {staff_supervisor.LastName} " +
                        $"after you complete your registration" +   
                        "</ol>" +
                        $"Kindly click on the link <a href = {url}> here </a> to register.");
                        return $"Employee ({User.EmployeeStatus}) Registration to {User.Email} sent successfully";
                    }
                    await _emailsender.SendEmailAsync(
                        User.Email,
                        "Congratulations on Joining the TX Family",
                        $"<h1>Welcome to TX Spare Parts, as a {User.EmployeeStatus} from {company.Name} " +
                        $"So basically your role as a {User.EmployeeStatus} is: </n>" +
                        "<ol>" +
                        "<li>Manages the staff of the company.</li>" +
                        "<li>Handles the employment of staff into the company.</li>" +
                        "<li>Manages all company products.</li>" +
                        "<li> Management of company orders and assists staff when the need arises</li>" +
                        "e.t.c" +
                        "</ol>" +
                        $"Kindly click on the link <a href = {url}> here </a> to register.");
                    return $"Employee({User.EmployeeStatus}) Registration to {User.Email} sent successfully";
                }
            }
            throw new BusinessException("The email was not able to send successfully");
        }

        public async Task<string> SendTokenService(ApplicationUser User)
        {
            var user = await _usermanager.FindByEmailAsync(User.Email);
            if (user != null)
            {
                user.PhoneNumber = User.PhoneNumber;
                await _usermanager.UpdateAsync(user);
                var send_reponse = await _phonenum.SendToken(user.PhoneNumber);  
                return send_reponse;
            }
            throw new BusinessException("The token could not be sent successfully");   
        }

        public async Task<string> UpdateUserService(UpdateUserDTO User, UpdateUserQueryFilter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.Id);
            if(user != null)
            {
                if(User.Email != null)
                {
                    var new_email_token = await _usermanager.GenerateChangeEmailTokenAsync(user, User.Email);
                    await _usermanager.ChangeEmailAsync(user, User.Email, new_email_token);
                    user.EmailConfirmed = false;
                }
                if(User.PhoneNumber != null)
                {
                    var phone_token = await _usermanager.GenerateChangePhoneNumberTokenAsync(user, User.PhoneNumber);
                    await _usermanager.ChangePhoneNumberAsync(user, User.PhoneNumber, phone_token);
                    user.PhoneNumberConfirmed = false;
                }
                if(await _usermanager.IsInRoleAsync(user, SD.Customer))
                {
                    if(User.FirstName != null)
                    {
                        user.FirstName = User.FirstName;
                    }
                    if(User.LastName != null)
                    {
                        user.LastName = User.LastName;
                    }
                    if (User.PhysicalAddress != null)
                    {
                        user.PhysicalAdress = User.PhysicalAddress;
                    }
                    if (User.DigitalAddress != null)
                    {
                        user.DigitalAddress = User.DigitalAddress;
                    }
                    if (User.City != null)
                    {
                        user.City = User.City;
                    }
                    if (User.Region != null)
                    {
                        user.Region = User.Region;
                    }
                
                }
                if (User.CurrentPassword != null && User.NewPassword != null && User.ConfirmNewPassword != null)
                {
                    var correct_password = await _usermanager.CheckPasswordAsync(user, User.CurrentPassword);
                    if (correct_password)
                    {
                        if (User.NewPassword != User.ConfirmNewPassword)
                        {
                            throw new BusinessException("The New passwords do not match");
                        }
                        var result = await _usermanager.ChangePasswordAsync(user, User.CurrentPassword, User.NewPassword);
                        if (!result.Succeeded)
                        {
                            throw new BusinessException("The new password could not change successfully...");
                        }
                    }
                    throw new BusinessException("The current password yout entered is incorrect");
                }
                var update_result = await _usermanager.UpdateAsync(user);
                if (!update_result.Succeeded)
                {
                    throw new BusinessException("The User could not update successfully");
                }
                if (user.PhoneNumberConfirmed == false)
                {
                    await _phonenum.SendToken(User.PhoneNumber);
                      
                }
                if(user.EmailConfirmed == false)
                {
                    var url = await _emailconfirm.GenerateToken(user);
                    await _emailsender.SendEmailAsync(
                      User.Email,
                      "Email Update Successful",
                      $"Please click on the link to <a href={url}>confirm</a> your new email address" +
                      $"or copy link below: </n> {url}");
                }
                return "Your user profile has been updated successfully";
            }
            throw new BusinessException("The user does not exist");  
        }

        public async Task<string> VerifyTokenService(ApplicationUser user, string code)
        {
            var authenticated_user = await _usermanager.FindByEmailAsync(user.Email);
            if(authenticated_user != null)
            {
                var token_response  = await _phonenum.VerifyToken(authenticated_user, code);
                authenticated_user.PhoneNumberConfirmed = true;
                await _usermanager.UpdateAsync(authenticated_user);  
                return token_response;
            }
            throw new BusinessException("This is not the authenticated user");
        }
    }
}
