using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Utility
{
    public class EmailConfirmationHandler : IEmailConfirmationHandler
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IConfiguration _configuration;

        public EmailConfirmationHandler(
            UserManager<ApplicationUser> usermanager,
            IConfiguration configuration)
        {
            _usermanager = usermanager;
            _configuration = configuration;
        }
        public async Task<string> GenerateToken(ApplicationUser User)
        {
            var result = await _usermanager.GenerateEmailConfirmationTokenAsync(User);
            var result_bytes = Encoding.UTF8.GetBytes(result);
            var validEmailToken = WebEncoders.Base64UrlEncode(result_bytes);
            string url = $"{_configuration["AppUrl"]}/api/Identity/Identity/confirmemail?userId={User.Id}&token={validEmailToken}";  
            return url;
        }
    }
}
