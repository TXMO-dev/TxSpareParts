using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Responses;
using TxSpareParts.Utility;

namespace TxSpareParts.Areas.Identity.Controllers
{
    [Area("Identity")]   
    [Route("api/Identity/[controller]")]
    [ApiController]
    public class IdentityController : Controller   
    {
        private readonly IMapper _mapper;
        private readonly IIdentityService _identity;
        
        

        public IdentityController(
            IMapper mapper,
            IIdentityService identity)
        {
            _mapper = mapper;
            _identity = identity;
        }
       
        [HttpPost("registercustomeruser")]
        public async Task<IActionResult> RegisterCustomerUser([FromBody]RegisterDTO register)
        {
            if (register.Password != register.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var registered_user = _mapper.Map<ApplicationUser>(register);
            var result = await _identity.RegisterUserService(registered_user,SD.Customer);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [HttpPost("registeradminuser")]
        public async Task<IActionResult> registeradminuser([FromForm]AdminDTO register)        
        {
            if (register.Password != register.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var registered_user = _mapper.Map<ApplicationUser>(register);
            var result = await _identity.RegisterAdminService(registered_user);
            var response = new ApiResponse<string>(result);
            return Ok(response);  
        }
        
        [HttpPost("registeremployeeuser")]
        public async Task<IActionResult> registeremployeeuser([FromForm]EmployeeDTO register)
        {
            if (register.Password != register.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var registered_user = _mapper.Map<ApplicationUser>(register);
            var result = await _identity.RegisterEmploymentService(registered_user);   
            var response = new ApiResponse<string>(result); 
            return Ok(response);
        }


        [HttpPost("loginuser")]
        public async Task<IActionResult> LoginUser([FromBody]LoginDTO login)
        {
            var login_credentials = _mapper.Map<ApplicationUser>(login);
            var login_response = await _identity.LoginUserService(login_credentials);
            var response = new ApiResponse<string>(login_response);
            return Ok(response);
        }

        
        [HttpGet("confirmemail")]
        public async Task<IActionResult> confirmemail([FromQuery]EmailConfirmationQueryFilter filter)
        {
            if (filter.userId == null || filter.token == null)
                return BadRequest("userId and token cannot be null");

            var result = await _identity.ConfirmEmailService(filter);
            ViewBag.myresults = result;
            return View();
        }
   
        [HttpPost("forgotpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> forgotpassword([FromBody]ForgotPasswordDTO forgotpassword)
        {
            var email = _mapper.Map<ApplicationUser>(forgotpassword);
            var forgotpassword_response = await _identity.ForgotPasswordService(email);
            var response = new ApiResponse<string>(forgotpassword_response);
            return Ok(response);
        }

        [HttpPost("resetpassword")]   
        public async Task<IActionResult> resetpassword([FromForm]ResetPasswordDTO reset_password)
        {
            if (reset_password.Email == null || reset_password.Token == null)
                return BadRequest("email and token cannot be null");
            var results = await _identity.ResetPasswordService(reset_password);
            var response = new ApiResponse<string>(results);
            return Ok(response);
        }
                  
        [Authorize]
        [HttpPost("updateuser")]
        public async Task<IActionResult> updateuser([FromBody]UpdateUserDTO user, [FromQuery]UpdateUserQueryFilter filter)
        {
            var result = await _identity.UpdateUserService(user, filter);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("Me")]
        public async Task<IActionResult> Me([FromQuery]UpdateUserQueryFilter filter)
        {  
            var result = await _identity.GetMeProfileService(filter);
            var response = new ApiResponse<ApplicationUser>(result);
            return Ok(response); 
        }

        [Authorize]  
        [HttpPost("sendtoken")]
        public async Task<IActionResult> sendtoken([FromQuery]TokenQueryFilter filter, [FromBody]PhoneDTO phonenumber)
        {
            var user = _mapper.Map<ApplicationUser>(filter);
            user.PhoneNumber = phonenumber.PhoneNumber;
            var result = await _identity.SendTokenService(user);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize]
        [HttpPost("verifytoken")]
        public async Task<IActionResult> verifytoken([FromQuery]TokenQueryFilter filter, [FromBody]CodeDTO code)
        {
            var result = _mapper.Map<ApplicationUser>(filter);
            var token_response = await _identity.VerifyTokenService(result, code.Code);
            var response = new ApiResponse<string>(token_response);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]   
        [HttpPost("sendadminemployment")]
        public async Task<IActionResult> SendAdminRegistrationEmail([FromBody]AdminEmailDTO email)
        {
            var user = _mapper.Map<ApplicationUser>(email);
            var result = await _identity.SendAdminEmploymentEmail(user,email.companyname);
            var response = new ApiResponse<string>(result);
            return Ok(response);

        }
        [Authorize(Roles = SD.Admin)]
        [HttpPost("sendsupervisoremployment")]
        public async Task<IActionResult> SendSupervisorEmployment([FromBody]EmployeeEmailDTO employee)
        {
            var supervisor = _mapper.Map<ApplicationUser>(employee);
            var result = await _identity.SendEmploymentEmail(supervisor, employee.companyname, SD.Supervisor);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Admin)] 
        [HttpPost("sendstaffemployment")]
        public async Task<IActionResult> SendStaffEmployment([FromBody]EmployeeEmailDTO employee)   
        {
            var staff = _mapper.Map<ApplicationUser>(employee);
            var result = await _identity.SendEmploymentEmail(staff, employee.companyname, SD.Staff);
            var response = new ApiResponse<string>(result);                 
            return Ok(response);   
        }

        [HttpGet("googletoken")]
        public IActionResult GoogleAuth()
        {
            var properties = new AuthenticationProperties()
            {
                // actual redirect endpoint for your app
                RedirectUri = Url.Action(nameof(ExternalResponse)),
                Items =
                {
                    { "LoginProvider", "Google" },
                },
                AllowRefresh = true,

            };

            return Challenge(properties, "Google");  
        }

        [HttpGet("facebooktoken")]
        public IActionResult FacebookAuth()
        {
            var properties = new AuthenticationProperties()
            {
                // actual redirect endpoint for your app
                RedirectUri = Url.Action(nameof(ExternalResponse)),
                Items =
                {
                    { "LoginProvider", "Facebook" },
                },
                AllowRefresh = true,

            };

            return Challenge(properties, "Facebook");
        }

        [HttpGet("ExternalResponse")]
        public async Task<IActionResult> ExternalResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var claims = result.Principal.Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            });

            var nameidentifier = "";
            var email = "";
            var firstname = "";   
            var lastname = "";
           


         
            foreach(var item in claims)
            {
                if (item.Type == ClaimTypes.NameIdentifier)
                    nameidentifier = item.Value;

                if (item.Type == ClaimTypes.GivenName)
                    firstname = item.Value;

                if (item.Type == ClaimTypes.Surname)
                    lastname = item.Value;

                if (item.Type == ClaimTypes.Email)
                    email = item.Value;

            }

            var user = new ApplicationUser  
            {
                Id = nameidentifier,
                Email = email,
                FirstName = firstname,
                LastName = lastname,
                UserName = email,
                EmailConfirmed = true,
                isVerified = true
            };

            var existing_user = await _identity.CheckForUserById(nameidentifier);
            if (existing_user == null)
            {
                var registration_token = await _identity.RegisterUserSocialService(user);
                var reponse = new ApiResponse<string>(registration_token);
                return Ok(reponse);   
            };

            var login_response_token = await _identity.LoginSocial(existing_user.Email);  
            var response = new ApiResponse<string>(login_response_token); 
            return Ok(response);   
        }   
    }
}

