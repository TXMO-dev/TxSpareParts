using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
using Newtonsoft.Json;

namespace TxSpareParts.Areas.Customer.Controllers
{
    [Area("Customer")]  
    [Route("/api/Customer/[controller]")]

    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly IUserService _userservice;
        private readonly IMapper _mapper;
        private readonly ICompanyService _companyservice;
        private readonly IUriService _uriservice;

        public CustomerController(
            IUserService userservice,
            IMapper mapper,
            ICompanyService companyservice,
            IUriService uriservice)
        {
            _userservice = userservice;
            _mapper = mapper;
            _companyservice = companyservice;
            _uriservice = uriservice;
        }
        [Authorize(Roles = SD.Customer)]
        [HttpPost("togglelike")]
        public async Task<IActionResult> ToggleProductLike(ToggleLikeQueryFilter toggle)
        {
            var result = await _userservice.ToggleProductLike(toggle.userId, toggle.productId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("createproductreview")]
        public async Task<IActionResult> CreateProductReview(
            [FromQuery]CreateProductReviewQueryFilter filter, 
            [FromBody]ProductReviewDTO productreview)
        {
            var product_review = _mapper.Map<ProductReview>(productreview);
            var result = await _userservice.CreateProductReview(filter, product_review);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPatch("updateproductreview")]
        public async Task<IActionResult> UpdateProductReview(
            [FromQuery] CreateProductReviewQueryFilter filter,
            [FromBody] ProductReviewDTO productreview)
        {
            var product_update_review = _mapper.Map<ProductReview>(productreview);
            var result = await _userservice.UpdateProductReview(filter, product_update_review);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpDelete("deleteproductreview")]
        public async Task<IActionResult> DeleteProductReview([FromQuery]CreateProductReviewQueryFilter filter)
        {
            var result = await _userservice.DeleteProductReview(filter);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize]
        [HttpGet("getallproductsreviews")]
        public async Task<IActionResult> GetAllProductsReviews([FromQuery]ProductReviewQueryFilter filter)
        {
            var result = await _userservice.GetAllProductReviews(filter);
            string next_endpoint = result.HasNextPage ? $"api/Customer/Customer/getallproductreviews/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Customer/Customer/getallproductreviews/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
            var metadata = new Metadata
            {
                TotalCount = result.TotalCount,
                PageSize = result.PageSize,
                CurrentPage = result.CurrentPage,
                TotalPages = result.TotalPages,
                HasNextPage = result.HasNextPage,
                HasPreviousPage = result.HasPreviousPage,
                NextPageUrl = _uriservice.GetProductPaginationUri(filter.filter, Url.RouteUrl(next_endpoint)).ToString(),
                PreviousPageUrl = _uriservice.GetProductPaginationUri(filter.filter, Url.RouteUrl(prev_endpoint)).ToString()
            };
            var response = new ApiResponse<IList<ProductReview>>(result)
            {
                Metadata = metadata
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("createcompanyreview")]
        public async Task<IActionResult> CreateCompanyReviews([FromQuery]CompanyReviewQueryFilter filter, [FromBody]CompanyReviewDTO companyreview)
        {
            var company_review = _mapper.Map<CompanyReview>(companyreview);
            var result = await _companyservice.CreateCompanyReview(filter.userId, filter.companyId, company_review);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPatch("updatecompanyreview")]
        public async Task<IActionResult> UpdateCompanyReviews([FromQuery]CompanyReviewQueryFilter filter, [FromBody]CompanyReviewDTO companyreview)
        {
            var review_update = _mapper.Map<CompanyReview>(companyreview);
            var result = await _companyservice.UpdateCompanyReview(filter.userId, filter.companyId, review_update);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }  

        [Authorize(Roles = SD.Customer)]
        [HttpDelete("deletecompanyreviews")]
        public async Task<IActionResult> DeleteCompanyReviews([FromQuery]DeleteCompanyReviewQueryFilter filter)
        {
            var result = await _companyservice.DeleteCompanyReview(filter.userId, filter.companyreviewId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("togglecompanyreviewlike")]
        public async Task<IActionResult> ToggleCompanyReviewLike([FromQuery]ToggleCompanyReviewLikeQueryFilter filter)
        {
            var toggle_like = await _companyservice.ToggleCompanyReviewLike(filter.userId, filter.companyReviewId);
            var response = new ApiResponse<string>(toggle_like);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpGet("Getallcompanyreviews")]
        public async Task<IActionResult> GetAllCompanyReviews([FromQuery]GetCompanyReviewQueryFIlter filter)
        {
            var result = await _companyservice.GetAllCompanyReviews(filter);
            string next_endpoint = result.HasNextPage ? $"api/Customer/Customer/getallcompanyreviews/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Customer/Customer/getallcompanyreviews/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
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
            var response = new ApiResponse<PagedList<CompanyReview>>(result)
            {
                Metadata = metadata
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("toggleproductreviewlike")]
        public async Task<IActionResult> ToggleProductReviewLike([FromQuery]ProductReviewLikeQueryFilter filter)
        {
            var result = await _userservice.ToggleProductReviewLike(filter.userId, filter.productreviewid);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }
        [Authorize(Roles = SD.Customer)]
        [HttpPost("togglestarred")]
        public async Task<IActionResult> ToggleStarred([FromQuery]ToggleStarredQueryFilter filter)
        {
            var result = await _userservice.ToggleStarred(filter.userId, filter.companyId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpGet("getallstarredcompaniesofcustomer")]
        public async Task<IActionResult> GetStarredCompaniesOfCustomer([FromQuery]StarredCompaniesQueryFIlter filter)
        {
            var result = await _userservice.GetAllStarredCompaniesOfCustomer(filter);
            string next_endpoint = result.HasNextPage ? $"api/Customer/Customer/getallstarredcompaniesofcustomer/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Customer/Customer/getallstarredcompaniesofcustomer/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
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
            var response = new ApiResponse<PagedList<Company>>(result)
            {
                Metadata = metadata
            };
            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(metadata));
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("createshoppingcart")]
        public async Task<IActionResult> CreateShoppingCart([FromQuery]ShoppingCartQueryFilter filter, [FromBody]ShoppingCartDTO Quantity)
        {
            var result = await _userservice.CreateShoppingCart(filter.userId, filter.productId, Quantity.quantity);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpGet("gotoshoppingcart")]
        public async Task<IActionResult> GoToShoppingCart([FromQuery]string userId)
        {
            var result = await _userservice.GoToShoppingCart(userId);
            var response = new ApiResponse<Order>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpDelete("deleteshoppingcart")]
        public async Task<IActionResult> DeleteShoppingCart([FromQuery]DeleteShoppingCartQueryFilter filter)
        {
            var result = await _userservice.DeleteShoppingCart(filter.userId, filter.shoppingcartid);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPatch("updateshoppingcart")]
        public async Task<IActionResult> UpdateShoppingCart([FromQuery]DeleteShoppingCartQueryFilter filter, [FromBody]ShoppingCartDTO Quantity)
        {
            var result = await _userservice.UpdateShoppingCart(filter.userId, filter.shoppingcartid, Quantity.quantity);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("placeorder")]
        public async Task<IActionResult> PlaceOrder([FromQuery]PlaceOrderQueryFilter filter)
        {
            var result = await _userservice.PlaceOrder(filter.userId, filter.orderId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("buynow")]
        public async Task<IActionResult> BuyNow([FromQuery]ShoppingCartQueryFilter filter, ShoppingCartDTO Quantity)
        {
            var result = await _userservice.BuyNow(filter.userId, filter.productId, Quantity.quantity);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet("verifytransaction")]
        public async Task<IActionResult> VerifyTransaction([FromQuery]VerifyTransactionQueryFilter filter,[FromBody]bool liketosave)
        {
            var result = await _userservice.VerifyTransactionService(filter.Userid, filter.Refferencecode,liketosave);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles =SD.Customer)]
        [HttpPost("chargeproductwithsavedcard")]
        public async Task<IActionResult> ChargeProductWithSavedCard([FromQuery]ChargeSavedCardBuyNowQueryFilter filter)
        {
            var result = await _userservice.ChargeSavedCardBuyNow(filter.userId, filter.productId, filter.signature,filter.quantity);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("chargeorderwithsavedcard")]
        public async Task<IActionResult> ChargeOrderWithSavedCard([FromQuery]ChargeSavedCardOrderQueryFilter filter)
        {
            var result = await _userservice.ChargeSavedCardOrder(filter.userId, filter.orderId, filter.signature);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }

        [Authorize(Roles = SD.Customer)]
        [HttpPost("getallsuccessfulpurchasesfromuser")]
        public async Task<IActionResult> GetAllSuccessfulPurchasesFromUser([FromQuery]StarredCompaniesQueryFIlter filter)
        {
            var result = await _userservice.GetAllUserPurchases(filter);
            string next_endpoint = result.HasNextPage ? $"api/Customer/Customer/getallsuccessfulpurchasesfromuser/pageNumber={result.CurrentPage + 1}&pageSize={result.PageSize}" : null;
            string prev_endpoint = result.HasPreviousPage ? $"api/Customer/Customer/getallsuccessfulpurchasesfromuser/pageNumber={result.CurrentPage - 1}&pageSize={result.PageSize}" : null;
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
            var response = new ApiResponse<PagedList<Product>>(result)
            {
                Metadata = metadata
            };
            return Ok(response);
        }

        [Authorize]
        [HttpPost("uploaduserimage")]
        public async Task<IActionResult> UploadUserImage([FromQuery]string userId,FileUpload file)
        {
            var result = await _userservice.UploadUserImage(userId, file);
            var response = new ApiResponse<string>(result);
            return Ok(response);   
        }

        [Authorize]
        [HttpDelete("deleteuserimage")]
        public async Task<IActionResult> DeleteUserImage([FromQuery]DeleteUserImageQueryFilter filter)
        {
            var result = await _userservice.DeleteUserImage(filter.userId, filter.userImageId);
            var response = new ApiResponse<string>(result);
            return Ok(response);
        }


    }
}
