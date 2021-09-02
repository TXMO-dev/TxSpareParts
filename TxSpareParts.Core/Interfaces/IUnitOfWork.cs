using System;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        //IApplicationUserRepository ApplicationUserRepository { get; }
        ICompanyRepository CompanyRepository { get; }
        ICompanyReviewRepository CompanyReviewRepository { get; }
        ILikeRepository LikeRepository { get; }
        IOrderRepository OrderRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductReviewRepository ProductReviewRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IStarRepository StarRepository { get; }
        IProductReviewLikeRepository ProductReviewLikeRepository { get; }    
        ICompanyReviewLikeRepository CompanyReviewLikeRepository { get; } 
        ICardDetailRepository CardDetailRepository { get; }
        IReceiptRepository ReceiptRepository { get; }
        IProductImageRepository ProductImageRepository { get; }
        ICompanyImageRepository CompanyImageRepository { get; }
        IUserImageRepository UserImageRepository { get; }
        Task Save();
    }
}
