using System;
using System.Threading.Tasks;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.Data;

namespace TxSpareParts.Infastructure.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public UnitOfWork(ApplicationDbContext db)
        {
            _db = db;
            
            CompanyRepository = new CompanyRepository(db);
            CompanyReviewRepository = new CompanyReviewRepository(db);
            LikeRepository = new LikeRepository(db);
            OrderRepository = new OrderRepository(db);
            ProductRepository = new ProductRepository(db);
            ProductReviewRepository = new ProductReviewRepository(db);
            ShoppingCartRepository = new ShoppingCartRepository(db);
            StarRepository = new StarRepository(db);
            ProductReviewLikeRepository = new ProductReviewLikeRepository(db);
            CompanyReviewLikeRepository = new CompanyReviewLikeRepository(db);
            CardDetailRepository = new CardDetailRepository(db);
            ReceiptRepository = new ReceiptRepository(db);
            ProductImageRepository = new ProductImageRepository(db);
            CompanyImageRepository = new CompanyImageRepository(db);
            UserImageRepository = new UserImageRepository(db);   
        }
        
        public ICompanyRepository CompanyRepository { get; private set; }
        public ICompanyReviewRepository CompanyReviewRepository { get; private set; }
        public ILikeRepository LikeRepository { get; private set; }
        public IOrderRepository OrderRepository { get; private set; }
        public IProductRepository ProductRepository { get; private set; }         
        public IProductReviewRepository ProductReviewRepository { get; private set; }
        public IShoppingCartRepository ShoppingCartRepository { get; private set; }
        public IStarRepository StarRepository { get; private set; }
        public IProductReviewLikeRepository ProductReviewLikeRepository { get; private set; }
        public ICompanyReviewLikeRepository CompanyReviewLikeRepository { get; private set; }
        public ICardDetailRepository CardDetailRepository { get; private set; }
        public IReceiptRepository ReceiptRepository { get; private set; }
        public IProductImageRepository ProductImageRepository { get; private set; }
        public ICompanyImageRepository CompanyImageRepository { get; private set; }
        public IUserImageRepository UserImageRepository { get; private set; }   

        public async void Dispose()
        {
            await _db.DisposeAsync();   
        }

        public async Task Save()
        {
            await _db.SaveChangesAsync();
        }
    }
}
