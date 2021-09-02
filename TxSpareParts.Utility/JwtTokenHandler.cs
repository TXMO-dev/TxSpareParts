using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Utility
{
    public class JwtTokenHandler : IJwtTokenHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _usermanager;
        public JwtTokenHandler(
            IConfiguration configuration,
            UserManager<ApplicationUser> usermanager)
        {
            _configuration = configuration;
            _usermanager = usermanager;
        }
        

        public async Task<string> GenerateToken(ApplicationUser user)
        {

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Providers:Key"]));
            var Claims = new List<Claim>
            {
                new Claim("Email",user.Email),    
                new Claim(ClaimTypes.NameIdentifier, user.Id),    
                new Claim("FirstName",user.FirstName),
                new Claim("LastName",user.LastName),
            };
            if(user.AdministrativeStatus == SD.ChiefAdmin)
            {
                Claims.Add(new Claim("AdminStatus", user.AdministrativeStatus));    
            }
            if(user.EmployeeStatus == SD.Supervisor)
            {
                Claims.Add(new Claim("SupervisorStatus", user.EmployeeStatus));
            }
            if(user.EmployeeStatus == SD.Staff)
            {
                Claims.Add(new Claim("StaffStatus", user.EmployeeStatus));
            }   
            var roles = await _usermanager.GetRolesAsync(user);
            AddRolesToClaims(Claims, roles);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Providers:validIssuer"],
                audience: _configuration["JWT:Providers:validAudience"],
                claims: Claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private void AddRolesToClaims(List<Claim> claims, IEnumerable<string> roles)
        {
            foreach (var role in roles)
            {
                var roleClaim = new Claim(ClaimTypes.Role, role);
                claims.Add(roleClaim);
            }
        }
    }
}
