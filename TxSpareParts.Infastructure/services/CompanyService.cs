using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Utility.Interfaces;
using TxSpareParts.Core.QueryFilter;

namespace TxSpareParts.Infastructure.services
{
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IEmailSender _emailsender;
        private readonly IPaymentProcessingHandler _payment;
        private readonly IImageHandler _imagehandler;
        public CompanyService(
            IUnitOfWork unitofwork,
            UserManager<ApplicationUser> usermanager,
            IEmailSender emailsender,
            IPaymentProcessingHandler payment,
            IImageHandler imagehandler)
        {
            _unitofwork = unitofwork;
            _usermanager = usermanager;
            _emailsender = emailsender;
            _payment = payment;
            _imagehandler = imagehandler;
        }
        public async Task<string> AddCompanyService(Company company)
        {
            if(company != null)
            {
                var code = await _payment.ListBanks(company.SupportedBank);
                if(code.Contains("Bank_code"))
                {
                    var code_split = code.Split("|");
                    var bank_name = code_split[2];
                    company.SupportedBank = bank_name;  
                }
                if (!code.Contains("Bank_code"))
                {
                    return code;       
                }
                await _unitofwork.CompanyRepository.Add(company);
                await _unitofwork.Save();
                return "Company created successfully";
            }
            throw new BusinessException("The company could not be created successfully");  

        }
        public async Task<string> CreateCompanyReview(string userId, string companyId, CompanyReview companyreview)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == companyId);
                if(company != null)
                {
                    var company_reviews = await _unitofwork.CompanyReviewRepository.GetAll(e => e.CompanyID == company.ID);
                    foreach(var review in company_reviews)
                    {
                        if(review.UserID == user.Id)
                        {
                            throw new BusinessException("This user has already written a review under this company. " +
                                "you can delete or update existing review");
                        }
                    }
                    var company_review = new CompanyReview
                    {
                        UserID = user.Id,
                        CompanyID = company.ID,
                        Comment = companyreview.Comment,
                        Rating = companyreview.Rating
                    };
                    await _unitofwork.CompanyReviewRepository.Add(company_review);
                    await _unitofwork.Save();
                    return $"Company review for {company.Name} has been created successfully";
                }
                throw new BusinessException("The company does not exist");
            }
            throw new BusinessException("The user does not exist");
        }
        public async Task<string> UpdateCompanyReview(string userId, string companyId, CompanyReview companyreview)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == companyId);

            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (company != null)
                {
                    var company_reviews_for_company = await _unitofwork.CompanyReviewRepository.GetAll(e => e.CompanyID == company.ID);
                    foreach (var company_review in company_reviews_for_company)
                    {
                        if (company_review.UserID == user.Id)
                        {
                            if (companyreview != null)
                            {
                                _unitofwork.CompanyReviewRepository.Update(companyreview);
                            }
                            await _unitofwork.Save();
                        }
                    }
                    throw new BusinessException("There is no company review associated with this user that needs to be updated");
                }
                throw new BusinessException("This company does not exist");
            }
            throw new BusinessException("THis user either doesnt exist or is not a customer");
        }
        public async Task<string> DeleteCompanyReview(string userId, string companyreviewId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                var company_review = await _unitofwork.CompanyReviewRepository.GetFirstOrDefault(e => e.UserID == user.Id && e.Id == companyreviewId);
                if(company_review == null)
                {
                    throw new BusinessException("The company review does not exist");
                }
                _unitofwork.CompanyReviewRepository.Remove(company_review.Id);
                await _unitofwork.Save();

                var company_review_likes = await _unitofwork.CompanyReviewLikeRepository.GetAll(e => e.CompanyReviewID == company_review.Id);
                foreach(var comp in company_review_likes)
                {
                    _unitofwork.CompanyReviewLikeRepository.Remove(comp.Id);
                    await _unitofwork.Save();
                }
                return "Your company review has been deleted successfully";
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> DeleteCompanyService(string id)
        {
            var company = await _unitofwork.CompanyRepository.Get(id);
             _unitofwork.CompanyRepository.Remove(id);
            await _unitofwork.Save();
            if(company != null)
            {
                var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
                var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);

                foreach(var administrator in administrators)
                {
                    if(administrator.AdministrativeStatus == null && administrator.CompanyId == company.ID)
                    {
                        var result = await _usermanager.DeleteAsync(administrator);
                        if (result.Succeeded)
                        {
                            await _emailsender.SendEmailAsync(administrator.Email, "Account Deleted", $"<p>Your company {company.Name} </p>" +
                                $"<p>is no longer a member  of our platform</p>" +
                                $"with that being said we will like to thank you for being a part of this platform and we wish you all the best.");
                        }
                    }
                }

                foreach(var employee in employees)
                {
                    if(employee.CompanyId == company.ID)
                    {
                        var result = await _usermanager.DeleteAsync(employee);
                        if (result.Succeeded)
                        {
                            await _emailsender.SendEmailAsync(employee.Email, "Account Deleted", $"<p>Your company {company.Name} </p>" +
                                $"<p>is no longer a member  of our platform</p>" +
                                $"with that being said we will like to thank you for being a part of this platform and we wish you all the best.");
                        }
                    }
                }
                return $"{company.Name} was deleted successfully.";
            }
            throw new BusinessException("The company does not exist");
        }
        public async Task<IList<Company>> GetAllCompanies(Company filter)
        {
            IList<Company> all_companies = new List<Company>();
            var companies = await _unitofwork.CompanyRepository.GetAll();
            foreach(var company in companies)
            {
                all_companies.Add(company);
            }

            if(filter.Address != null)
            {
                IList<Company>address_results = new List<Company>();
                var address_companies = await _unitofwork.CompanyRepository.GetAll(x => x.Address == filter.Address);
                foreach(var address_company in address_companies)
                {
                    address_results.Add(address_company);
                }
                return address_results;
            }

            if (filter.City != null)
            {
                IList<Company> city_results = new List<Company>();
                var city_companies = await _unitofwork.CompanyRepository.GetAll(x => x.City == filter.City);
                foreach (var city_company in city_companies)
                {
                    city_results.Add(city_company);
                }
                return city_results;
            }

            if (filter.Region != null)
            {
                IList<Company> region_results = new List<Company>();
                var region_companies = await _unitofwork.CompanyRepository.GetAll(x => x.Region == filter.Region);
                foreach (var region_company in region_companies)
                {
                    region_results.Add(region_company);
                }
                return region_results;
            }

            if (filter.DigitalAddress != null)
            {
                IList<Company> digital_results = new List<Company>();
                var digital_companies = await _unitofwork.CompanyRepository.GetAll(x => x.DigitalAddress == filter.DigitalAddress);
                foreach (var digital_company in digital_companies)
                {
                    digital_results.Add(digital_company);
                }
                return digital_results;
            }

            if (filter.PostOfficeBox != null)
            {
                IList<Company> post_results = new List<Company>();
                var post_companies = await _unitofwork.CompanyRepository.GetAll(x => x.PostOfficeBox == filter.PostOfficeBox);
                foreach (var post_company in post_companies)
                {
                    post_results.Add(post_company);
                }
                return post_results;
            }

            if (filter.PhoneNumber != null)
            {
                IList<Company> phone_results = new List<Company>();
                var phone_companies = await _unitofwork.CompanyRepository.GetAll(x => x.PhoneNumber == filter.PhoneNumber);
                foreach (var phone_company in phone_companies)
                {
                    phone_results.Add(phone_company);
                }
                return phone_results;
            }
            if (filter.SecondPhoneNumber != null)
            {
                IList<Company> phone_results = new List<Company>();
                var phone_companies = await _unitofwork.CompanyRepository.GetAll(x => x.SecondPhoneNumber == filter.SecondPhoneNumber);
                foreach (var phone_company in phone_companies)
                {
                    phone_results.Add(phone_company);
                }
                return phone_results;
            }

            if (filter.ThirdPhoneNumber != null)
            {
                IList<Company> phone_results = new List<Company>();
                var phone_companies = await _unitofwork.CompanyRepository.GetAll(x => x.ThirdPhoneNumber == filter.ThirdPhoneNumber);
                foreach (var phone_company in phone_companies)
                {
                    phone_results.Add(phone_company);
                }
                return phone_results;
            }

            if (filter.Email != null)
            {
                IList<Company> phone_results = new List<Company>();
                var phone_companies = await _unitofwork.CompanyRepository.GetAll(x => x.Email == filter.Email);
                foreach (var phone_company in phone_companies)
                {
                    phone_results.Add(phone_company);
                }
                return phone_results;
            }

            if (filter.Name != null)
            {
                IList<Company> phone_results = new List<Company>();
                var phone_companies = await _unitofwork.CompanyRepository.GetAll(x => x.Name == filter.Name);
                foreach (var phone_company in phone_companies)
                {
                    phone_results.Add(phone_company);
                }
                return phone_results;
            }
            return all_companies;
        }
        public async Task<string> UpdateCompanyService(string id, Company company)
        {
            var found_company = await _unitofwork.CompanyRepository.Get(id);
            if(found_company != null)
            {
                if (company != null)
                {
                    found_company = company;
                }
                _unitofwork.CompanyRepository.Update(found_company);
                await _unitofwork.Save();
                return $"{found_company.Name} has been updated successfully";
            }
            throw new BusinessException("The selected company does not exist");  
        }
        public async Task<PagedList<CompanyReview>> GetAllCompanyReviews(GetCompanyReviewQueryFIlter filter)
        {
            var company = await _unitofwork.CompanyReviewRepository.GetFirstOrDefault(e => e.CompanyID == filter.companyId);
            if(company != null)
            {
                var all_companies_reviews = await _unitofwork.CompanyReviewRepository.GetAll(e => e.CompanyID == company.Id);
                if(all_companies_reviews != null)
                {
                    return PagedList<CompanyReview>.Create(all_companies_reviews, (int)filter.pagination.pageNumber,(int)filter.pagination.pageSize);
                }
                throw new BusinessException("The reviews for this company is empty");
            }
            throw new BusinessException("This company does not exist");
        }
        public async Task<string> ToggleCompanyReviewLike(string userId, string companyReviewId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            IList<CompanyReviewLike> unlike_total_list = new List<CompanyReviewLike>();
            if (user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))    
            {
                var company_review = await _unitofwork.CompanyReviewRepository.GetFirstOrDefault(e => e.Id == companyReviewId);
                var company = new Company();
                if(company_review != null)
                {
                    company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == company_review.CompanyID);
                }
                var company_review_likes = await _unitofwork.CompanyReviewLikeRepository.GetAll(e => e.CompanyReviewID == companyReviewId);
                if(company_review_likes != null)
                {
                    foreach(var review_like in company_review_likes)
                    {
                        if(review_like.UserID == user.Id)
                        {
                            _unitofwork.CompanyReviewLikeRepository.Remove(review_like.Id);
                            await _unitofwork.Save();
                            var unlike_total = await _unitofwork.CompanyReviewLikeRepository.GetAll(e => e.CompanyReviewID == review_like.CompanyReviewID);

                            if(unlike_total != null)
                            {
                                foreach(var unlike in unlike_total)
                                {
                                    unlike_total_list.Add(unlike);
                                }
                            }
                            return $"{unlike_total_list.Count} likes unliked by {user.FirstName} {user.LastName}";
                        }
                    }

                    var new_like = new CompanyReviewLike
                    {
                        UserID = user.Id,
                        CompanyID = company.ID,
                        CompanyReviewID = company_review.Id
                    };
                    await _unitofwork.CompanyReviewLikeRepository.Add(new_like);
                    await _unitofwork.Save();
                    var like_total = await _unitofwork.CompanyReviewLikeRepository.GetAll(e => e.CompanyReviewID == company_review.Id);
                    if (like_total != null)
                    {
                        foreach (var like in like_total)
                        {
                            unlike_total_list.Add(like);
                        }
                    }
                    return $"{unlike_total_list.Count} likes liked by {user.FirstName} {user.LastName}";

                }
            }
            throw new BusinessException("This user either does not exist or is not a customer");
        }
        public async Task<string> UploadCompanyImage(string companyId, FileUpload file)
        {
            var company = await _unitofwork.CompanyRepository.Get(companyId);
            if(company != null)
            {
                var all_company_images = await _unitofwork.CompanyImageRepository.GetAll(e => e.CompanyId == company.ID);
                var all_company_list = all_company_images.ToList();
                if(all_company_list.Count == 2)
                {
                    throw new BusinessException("The maximum image upload limit is 2, prefferably your company logo and store front");
                }
                var company_image_url = await _imagehandler.CreateImage(company.ID, SD.Company_type, file);
                if(company_image_url != null)
                {
                    await _unitofwork.CompanyImageRepository.Add(new CompanyImage
                    {
                        CompanyId = company.ID,
                        ImageUrl = company_image_url
                    });
                    await _unitofwork.Save();
                }
                throw new BusinessException("The company imany image url does not exist");
            }
            throw new BusinessException("The companyimage does not exist");
        }
        public async Task<string> DeleteCompanyImage(string companyId, string companyImageId)
        {
            var company = await _unitofwork.CompanyRepository.Get(companyId);
            if(company != null)
            {
                var company_image = await _unitofwork.CompanyImageRepository.Get(companyImageId);
                if(company_image != null)
                {
                    var all_company_images = await _unitofwork.CompanyImageRepository.GetAll(e => e.Id == company_image.Id);
                    var all_company_list = all_company_images.ToList();
                    if(all_company_list.Count == 1)
                    {
                        throw new BusinessException("The company image cannot be empty you will need to upload a new image and delete this one");
                    }
                    _unitofwork.CompanyImageRepository.Remove(company_image.Id);
                    await _unitofwork.Save();
                }
                throw new BusinessException("The company image does not exist");
            }
            throw new BusinessException("The company does not exist");
        }
    }
}
