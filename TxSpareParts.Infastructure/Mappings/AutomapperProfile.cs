using AutoMapper;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Mappings
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<RegisterDTO, ApplicationUser>();
            CreateMap<LoginDTO, ApplicationUser>();
            CreateMap<ForgotPasswordDTO, ApplicationUser>();
            CreateMap<TokenQueryFilter, ApplicationUser>();
            CreateMap<PhoneDTO, ApplicationUser>();
            CreateMap<AdminDTO, ApplicationUser>();
            CreateMap<AdminEmailDTO, ApplicationUser>();

            CreateMap<EmployeeEmailDTO, ApplicationUser>();
            CreateMap<EmployeeDTO, ApplicationUser>();

            CreateMap<AllCustomerStaffDTO, ApplicationUser>();

            CreateMap<CompanyDTO, Company>();

            CreateMap<ProductDTO, Product>();
            CreateMap<ProductNameDTO, Product>();

            CreateMap<ApplicationUser, UpdateUserQueryFilter>();

            CreateMap<ProductReviewDTO, ProductReview>();
            CreateMap<CompanyReviewDTO, CompanyReview>();

            CreateMap<OrderDTO, Order>();          
            
        }
    }
}
