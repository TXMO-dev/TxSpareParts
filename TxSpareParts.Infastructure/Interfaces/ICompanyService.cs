using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface ICompanyService
    {
        Task<string> AddCompanyService(Company company);
        Task<string> UpdateCompanyService(string id, Company company);
        Task<string> DeleteCompanyService(string id);
        Task<IList<Company>> GetAllCompanies(Company filter);
        Task<string> CreateCompanyReview(string userId, string companyId, CompanyReview companyreview);
        Task<string> UpdateCompanyReview(string userId, string companyId, CompanyReview companyreview);
        Task<string> DeleteCompanyReview(string userId, string companyreviewId);
        Task<string> ToggleCompanyReviewLike(string userId, string companyReviewId);
        Task<string> UploadCompanyImage(string companyId, FileUpload file);
        Task<string> DeleteCompanyImage(string companyId, string companyImageId);
        Task<PagedList<CompanyReview>> GetAllCompanyReviews(GetCompanyReviewQueryFIlter filter);
    }
}
