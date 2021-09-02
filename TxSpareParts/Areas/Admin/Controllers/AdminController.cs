using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Responses;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/Admin/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly ICompanyService _companyservice;
        private readonly IMapper _mapper;
        private readonly IUserService _userservice;
        private readonly IAdminService _adminservice;
        private readonly IIdentityService _identity;
        private readonly IUriService _uriservice;

        public AdminController(
            ICompanyService companyservice,
            IMapper mapper,
            IUserService userservice,
            IAdminService adminservice,
            IIdentityService identity,
            IUriService uriservice)
        {
            _companyservice = companyservice;
            _mapper = mapper;
            _userservice = userservice;
            _adminservice = adminservice;
            _identity = identity;
            _uriservice = uriservice;
        }


        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPost("createcompany")]
        public async Task<IActionResult> CreateCompany([FromBody]CompanyDTO company)
        {
            var registered_company = _mapper.Map<Company>(company);
            var result = await _companyservice.AddCompanyService(registered_company);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpGet("getallcompanies")]
        public async Task<IActionResult> GetAllCompanies([FromBody]CompanyDTO company)
        {
            var filter = _mapper.Map<Company>(company);
            var result = await _companyservice.GetAllCompanies(filter);
            var response = new ApiResponse<IList<Company>>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPatch("updatecompany")]
        public async Task<IActionResult> UpdateCompany([FromQuery]string id, [FromBody]CompanyDTO company_details)
        {
            var company = _mapper.Map<Company>(company_details);
            var result = await _companyservice.UpdateCompanyService(id, company);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpGet("getchiefstaff")]
        public async Task<IActionResult> GetChiefUsers([FromBody]AllChiefStaffDTO chiefstaff)
        {
            var chief_results = await _userservice.GetAllChiefStaff(chiefstaff);
            var response = new ApiResponse<IEnumerable<ApplicationUser>>(chief_results);
            return Ok(response);
        }
        [Authorize(Policy = "ChiefAuthorization")]
        [HttpDelete("deletecompanybychief")]
        public async Task<IActionResult> DeleteCompanyByChief([FromQuery]string id)
        {
            var result = await _companyservice.DeleteCompanyService(id);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Admin)]
        [HttpGet("getadminstaff")]
        public async Task<IActionResult> GetAdminUsers([FromBody]AllAdminStaffDTO adminstaff)
        {
            var admin_results = await _userservice.GetAllAdminStaff(adminstaff);
            var response = new ApiResponse<IEnumerable<ApplicationUser>>(admin_results);
            return Ok(response);

        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPost("lockoutwithchief")]
        public async Task<IActionResult> LockoutWithChief([FromQuery]string id,[FromBody]SupervisorDTO assigntosupervisor)
        {
            var result = await _adminservice.LockoutServiceForChief(id, assigntosupervisor);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Admin)]
        [HttpPost("lockoutwithadmin")]
        public async Task<IActionResult> LockoutWithAdmin([FromQuery]string id, [FromBody]SupervisorDTO assigntosupervisor)
        {
            var result = await _adminservice.LockoutServiceForAdmin(id, assigntosupervisor);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPatch("updateadminbychief")]
        public async Task<IActionResult> UpdateAdminByChief([FromQuery]string id, [FromBody]UpdateUserDTO user)
        {
            string[] roles = new[]{SD.Admin};   
            var result = await _adminservice.UpdateService(id, user, roles);
            var response = new ApiResponse<string>(result);
            return Ok(response);  
        }

        [Authorize(Roles = SD.Admin)]
        [HttpPatch("updateemployeebyadmin")]
        public async Task<IActionResult> UpdateEmployeeByAdmin([FromQuery]string id, [FromBody]UpdateUserDTO user)
        {
            string[] roles = new[]{SD.Employee};
            var result = await _adminservice.UpdateService(id, user, roles);   
            var response = new ApiResponse<string>(result);
            return Ok(response);

        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpDelete("deleteadministrationforchief")]
        public async Task<IActionResult> DeleteAdministrationForChief([FromQuery]string id)
        {
            var result = await _adminservice.DeleteAdministratorForChief(id);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Admin)]
        [HttpDelete("deletesupervisororemployee")]
        public async Task<IActionResult> DeleteSupervisorOrEmployee([FromQuery]string id, [FromBody]SupervisorDTO assignedsupervisor)
        {
            var result = await _adminservice.DeleteStaff(id, assignedsupervisor);
            var response = new ApiResponse<string>(result);
            return Ok(response);   
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPatch("updatecustomerbychief")]
        public async Task<IActionResult> UpdateCustomerByChief([FromQuery]string id, [FromBody]UpdateUserDTO user)
        {
            string[] roles = new[] { SD.Customer };
            var result = await _adminservice.UpdateService(id, user, roles);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPost("createanyuserbychief")]
        public async Task<IActionResult> CreateAnyUserByChief([FromBody]RegisterDTO register)
        {
            var user = _mapper.Map<ApplicationUser>(register);
            var result = await _identity.RegisterUserService(user, user.role);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpGet("getallcustomerbychief")]
        public async Task<IActionResult> GetAllCustomerByChief([FromBody]AllCustomerStaffDTO customer)
        {
            var customer_user = _mapper.Map<ApplicationUser>(customer);
            var result = await _userservice.GetAllCustomerStaff(customer_user);
            var response = new ApiResponse<IEnumerable<ApplicationUser>>(result);
            return Ok(response);   
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpDelete("deletecustomerbychief")]
        public async Task<IActionResult> DeleteCustomerByChief([FromQuery]string id)
        {
            var result = await _adminservice.DeleteCustomer(id);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpPost("uploadcompanyimages")]
        public async Task<IActionResult> UploadCompanyImages([FromQuery]string companyId, FileUpload file)
        {
            var result = await _companyservice.UploadCompanyImage(companyId, file);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpDelete("deletecompanyimages")]
        public async Task<IActionResult> DeleteCompanyImage([FromQuery]CompanyImageQueryFilter filter)
        {
            var result = await _companyservice.DeleteCompanyImage(filter.companyId, filter.companyImageId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpGet("getallchieforders")]
        public async Task<IActionResult> GetAllChiefOrders([FromQuery]StarredCompaniesQueryFIlter filter)
        {
            var result = await _adminservice.GetAllChiefOrdersService(filter);
            string next_endpoint = result.HasNextPage ? $"api/Admin/Admin/getallchieforders/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Admin/Admin/getallchieforders/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
            var metadata = new Metadata
            {
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage,
                NextPageUrl = _uriservice.GetProductPaginationUri(filter.pagination, Url.RouteUrl(next_endpoint)).ToString(),
                PreviousPageUrl = _uriservice.GetProductPaginationUri(filter.pagination, Url.RouteUrl(prev_endpoint)).ToString()
            };
            var response = new ApiResponse<PagedList<Order>>(result)
            {
                Metadata = metadata
            };  
            return Ok(response);   
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPatch("updatechieforders")]
        public async Task<IActionResult> UpdateChiefOrders([FromQuery]string userId, [FromQuery]string orderId, OrderDTO order)
        {
            var updated_order = _mapper.Map<Order>(order);
            var result = await _adminservice.UpdateChiefOrderService(userId, orderId, updated_order);   
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpDelete("deletechieforders")]
        public async Task<IActionResult> DeleteChiefOrder([FromQuery]string userId, [FromQuery]string orderId)
        {
            var result = await _adminservice.DeleteChiefOrderService(userId, orderId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [HttpPost("handledisputesforchief")]
        public async Task<IActionResult> HandleDisputeForChief([FromQuery]string userId,[FromBody]HandleDisputeDTO dispute)
        {
            var result = await _adminservice.HandleDisputeChiefService(userId, dispute.From, dispute.To);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }   


    }
}               
