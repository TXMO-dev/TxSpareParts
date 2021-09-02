using System.Collections.Generic;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllChiefStaff(AllChiefStaffDTO User);
        Task<IEnumerable<ApplicationUser>> GetAllAdminStaff(AllAdminStaffDTO User);
        Task<IEnumerable<ApplicationUser>> GetAllCustomerStaff(ApplicationUser User);
        Task<string> ToggleProductLike(string userId, string productId);
        Task<string> CreateProductReview(CreateProductReviewQueryFilter filter, ProductReview productreview);
        Task<string> DeleteProductReview(CreateProductReviewQueryFilter filter);
        Task<string> UpdateProductReview(CreateProductReviewQueryFilter filter, ProductReview productreview);
        Task<PagedList<ProductReview>> GetAllProductReviews(ProductReviewQueryFilter filter);
        Task<string> ToggleProductReviewLike(string userId, string productreviewId);
        Task<string> ToggleStarred(string userId, string companyId);
        Task<PagedList<Company>> GetAllStarredCompaniesOfCustomer(StarredCompaniesQueryFIlter filter);   
        Task<string> CreateShoppingCart(string userId, string productId, int quantity);
        Task<Order> GoToShoppingCart(string userId);
        Task<string> DeleteShoppingCart(string userId, string shoppingCartId);
        Task<string> UpdateShoppingCart(string userId, string shoppingCartId, int quantity);
        Task<string> PlaceOrder(string userId, string OrderId);
        Task<string> BuyNow(string userId, string productId, int quantity);
        Task<string> VerifyTransactionService(string userId, string refference,bool liketosave);
        Task<string> ChargeSavedCardBuyNow(string userId, string productId, string signature, int quantity);
        Task<string> ChargeSavedCardOrder(string userId, string orderId, string signature);
        Task<PagedList<Product>> GetAllUserPurchases(StarredCompaniesQueryFIlter filter);
        Task<string> UploadUserImage(string userId, FileUpload file);
        Task<string> DeleteUserImage(string userId, string imageId);
    

    }
}
