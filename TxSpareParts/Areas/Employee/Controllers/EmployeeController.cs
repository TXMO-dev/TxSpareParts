using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Responses;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Core.CustomEntities;

namespace TxSpareParts.Areas.Employee.Controllers
{
    [Area("Employee")]  
    [Route("api/Employee/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeservice;
        private readonly IMapper _mapper;
        private readonly IProductService _productservice;
        private readonly IUriService _uriservice;


        public EmployeeController(
            IEmployeeService employeeservice,
            IMapper mapper,
            IProductService productservice,
            IUriService uriservice)
        {
            _mapper = mapper;
            _employeeservice = employeeservice;
            _productservice = productservice;
            _uriservice = uriservice;
        }

        [Authorize(Policy = "SupervisorAuthorisation")]
        [HttpGet("getsupervisorstaff")]
        public async Task<IActionResult> GetSuperVisorStaff([FromBody]AllSupervisorDTO filter, [FromQuery]GetSupervisorStaffQueryFilter user)
        {
            var employee_result = await _employeeservice.GetAllSupervisorStaff(filter, user.Id);
            var response = new ApiResponse<IEnumerable<ApplicationUser>>(employee_result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpPost("createproduct")]
        public async Task<IActionResult> CreateProduct([FromQuery]string id, [FromBody]ProductDTO product)
        {
            var sparepart = _mapper.Map<Product>(product);
            var result = await _productservice.CreateProduct(id, sparepart);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpPatch("updateproduct")]
        public async Task<IActionResult> UpdateProduct([FromQuery]UpdateProductQueryFilter filter, [FromBody]ProductDTO product)
        {
            var updated_product = _mapper.Map<Product>(product);
            var result = await _productservice.UpdateProduct(filter.id, filter.productid, updated_product);
            var response = new ApiResponse<string>(result);
            return Ok(response);   
        }

        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [Authorize(Roles = SD.Customer)]
        [HttpGet("readandfiterallproducts")]
        public async Task<IActionResult> ReadAndFilterAllProducts([FromQuery]string id,[FromQuery]PaginationQueryFIlter filter, [FromBody]ProductNameDTO product)
        {
            var product_filter = _mapper.Map<Product>(product);
            var result = await _productservice.ReadAndFilterAllProducts(id, filter.pageNumber, filter.pageSize, product_filter);
            string next_endpoint = result.HasNextPage ? $"api/Employee/Employee/readandfilterallproducts/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Employee/Employee/readandfilterallproducts/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
            var metadata = new Metadata
            {
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage,
                NextPageUrl = _uriservice.GetProductPaginationUri(filter, Url.RouteUrl(next_endpoint)).ToString(),
                PreviousPageUrl = _uriservice.GetProductPaginationUri(filter, Url.RouteUrl(prev_endpoint)).ToString()   
            };
            var response = new ApiResponse<IList<Product>>(result)
            {
                Metadata = metadata
            };
            Response.Headers.Add("X-Pagination",JsonConvert.SerializeObject(metadata));
            return Ok(response);
        }

        [Authorize]
        [HttpGet("readproduct")]
        public async Task<IActionResult> ReadProduct([FromQuery]string id)
        {
            var result = await _productservice.ReadProduct(id);
            var response = new ApiResponse<Product>(result);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpDelete("deleteproduct")]
        public async Task<IActionResult> DeleteProduct([FromQuery]string id)
        {
            var deleted_product = await _productservice.DeleteProduct(id);
            var response = new ApiResponse<string>(deleted_product);
            return Ok(response);
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpPost("uploadproductimages")]
        public async Task<IActionResult> UploadProductImages([FromQuery]string productId,FileUpload file)
        {
            var result = await _productservice.UploadProductImage(productId, file);
            var response = new ApiResponse<string>(result);
            return Ok(response);   
        }

        [Authorize(Policy = "ChiefAuthorization")]
        [Authorize(Policy = "StaffAuthorization")]
        [Authorize(Policy = "SupervisorAuthorization")]
        [Authorize(Roles = SD.Admin)]
        [HttpDelete("deleteproductimages")]
        public async Task<IActionResult> DeleteProductImage([FromQuery]ProductImageQueryFilter filter)
        {
            var result = await _productservice.DeleteProductImage(filter.productId, filter.productimageId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }      
    }
}
