using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IIdentityService
    {
        Task<string> RegisterUserService(ApplicationUser User, string role);
        Task<string> RegisterUserSocialService(ApplicationUser User);
        Task<string> LoginUserService(ApplicationUser User);
        Task<string> ConfirmEmailService(EmailConfirmationQueryFilter filter);
        Task<string> ForgotPasswordService(ApplicationUser User);
        Task<string> ResetPasswordService(ResetPasswordDTO reset_password);
        Task<string> LoginSocial(string email);
        Task<ApplicationUser> CheckForUserById(string id);
        Task<string> VerifyTokenService(ApplicationUser user, string code);
        Task<string> SendTokenService(ApplicationUser User);
        Task<string> SendAdminEmploymentEmail(ApplicationUser User, string companyname);   
        Task<string> SendEmploymentEmail(ApplicationUser User, string companyname, string employeestatus);
        Task<string> RegisterEmploymentService(ApplicationUser User);        
        Task<string> RegisterAdminService(ApplicationUser User);
        Task<string> UpdateUserService(UpdateUserDTO User, UpdateUserQueryFilter filter);
        Task<ApplicationUser> GetMeProfileService(UpdateUserQueryFilter filter);

        
    }
}
