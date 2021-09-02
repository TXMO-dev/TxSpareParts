using System.Collections.Generic;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IProductService
    {
        Task<string> CreateProduct(string id,Product product);
        Task<string> UpdateProduct(string id, string productId, Product product);
        Task<Product> ReadProduct(string id);
        Task<PagedList<Product>> ReadAndFilterAllProducts(string id, int? pageNumber, int? pageSize, Product product);
        Task<string> DeleteProduct(string id);
        Task<string> UploadProductImage(string productId, FileUpload file);  
        Task<string> DeleteProductImage(string productId, string productImageId);
    }
}
