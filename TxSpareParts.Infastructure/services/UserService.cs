using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;
using TxSpareParts.Utility.Interfaces;

namespace TxSpareParts.Infastructure.services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IUnitOfWork _unitofwork;
        private readonly IPaymentProcessingHandler _paymenthandler;
        private readonly IImageHandler _imagehandler;
        private readonly IInvoiceHandler _invoice;
        private readonly IEmailSender _emailsender;
        public UserService(
            UserManager<ApplicationUser> usermanager,
            IUnitOfWork unitofwork,
            IPaymentProcessingHandler paymenthandler,
            IImageHandler imagehandler,
            IInvoiceHandler invoice,
            IEmailSender emailsender)
        {
            _usermanager = usermanager;
            _unitofwork = unitofwork;
            _paymenthandler = paymenthandler;
            _imagehandler = imagehandler;
            _invoice = invoice;
            _emailsender = emailsender;
        }

        public async Task<string> ToggleProductLike(string userId, string productId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            var product = await _unitofwork.ProductRepository.Get(productId);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == product.CompanyID);
            var all_likes_for_product = await _unitofwork.LikeRepository.GetAll(e => e.productID == productId);
            if (all_likes_for_product == null)
            {
                return "0";
            }
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                IList<Like> some_likes = new List<Like>();
                foreach (var like in all_likes_for_product)
                {
                    if (like.userID == userId)
                    {

                        some_likes.Add(like);
                    }
                }
                if (some_likes.Count > 0)
                {
                    _unitofwork.LikeRepository.Remove(some_likes[0].Id);
                    await _unitofwork.Save();
                    var remove_total = await _unitofwork.LikeRepository.GetAll(e => e.productID == productId);
                    IList<Like> total_remove = null;
                    foreach (var remove in remove_total)
                    {
                        total_remove.Add(remove);
                    }
                    return $"{total_remove.Count} unliked by {user.FirstName} {user.LastName}";
                }

                var Likes = new Like
                {
                    productID = productId,
                    userID = user.Id
                };

                await _unitofwork.LikeRepository.Add(Likes);
                await _unitofwork.Save();
                var add_total = await _unitofwork.LikeRepository.GetAll(e => e.productID == productId);
                IList<Like> total_add = new List<Like>();
                foreach (var add in add_total)
                {
                    total_add.Add(add);
                }
                return $"{total_add.Count} liked by {user.FirstName} {user.LastName}";
            }
            throw new BusinessException("Only customers are allowed to like products");

        }
        public async Task<IEnumerable<ApplicationUser>> GetAllAdminStaff(AllAdminStaffDTO User)
        {
            var employee_users = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            var company = new Company();
            if (User.CompanyName != null)
            {
                company = await _unitofwork.CompanyRepository
                                            .GetFirstOrDefault(e =>
                                            e.Name == User.CompanyName ||
                                            e.Name.Contains(User.CompanyName));
            }


            IList<ApplicationUser> supervisor_list = new List<ApplicationUser>();
            IList<ApplicationUser> staff_list = new List<ApplicationUser>();
            IList<ApplicationUser> supervisor_staff_list = new List<ApplicationUser>();
            foreach (var employee in employee_users)
            {
                if (employee.EmployeeStatus == SD.Supervisor)
                {
                    supervisor_list.Add(employee);
                }
                if (employee.AssignedTo != null && employee.EmployeeStatus == SD.Staff)
                {
                    staff_list.Add(employee);
                }

                if (User != null)
                {
                    if (employee.CompanyId == company.ID ||
                      employee.FirstName == User.FirstName ||
                      employee.FirstName.Contains(User.FirstName) ||
                      employee.LastName == User.LastName ||
                      employee.LastName.Contains(User.LastName) ||
                      employee.EmployeeStatus == User.EmployeeStatus ||
                      employee.PhoneNumber == User.PhoneNumber ||
                       employee.PhoneNumber.Contains(User.PhoneNumber))
                    {

                        if (User.StaffUnderSupervisorFirstName != null || User.StaffUnderSupervisorLastName != null)
                        {
                            foreach (var staff in staff_list)
                            {
                                foreach (var supervisor in supervisor_list)
                                {

                                    if (
                                       User.StaffUnderSupervisorFirstName == supervisor.FirstName ||
                                       supervisor.FirstName.Contains(User.StaffUnderSupervisorFirstName) ||
                                       supervisor.LastName == User.StaffUnderSupervisorLastName ||
                                       supervisor.LastName.Contains(User.StaffUnderSupervisorLastName))
                                    {
                                        if (staff.AssignedTo == supervisor.Id)
                                        {
                                            supervisor_staff_list.Add(staff);
                                        }
                                    }

                                }
                            }
                            if (supervisor_staff_list.Count != 0)
                            {
                                return supervisor_staff_list;
                            }
                        }
                        return employee_users;
                    }

                    if (User.supervisorsonly)
                    {
                        return supervisor_list;
                    }
                    if (User.staffonly)
                    {
                        return staff_list;
                    }
                    return employee_users;
                }
            }
            return employee_users;

        }
        public async Task<IEnumerable<ApplicationUser>> GetAllChiefStaff(AllChiefStaffDTO User)
        {
            var admin_users = await _usermanager.GetUsersInRoleAsync(SD.Admin);
            var customer_users = await _usermanager.GetUsersInRoleAsync(SD.Customer);
            var employee_users = await _usermanager.GetUsersInRoleAsync(SD.Employee);

            IList<ApplicationUser> all_users = new List<ApplicationUser>();

            if (admin_users != null)
            {
                foreach (var admin_user in admin_users)
                {
                    if (admin_user.AdministrativeStatus == null)
                    {
                        all_users.Add(admin_user);
                    }
                }
            }
            if (customer_users != null)
            {
                foreach (var customer_user in customer_users)
                {
                    all_users.Add(customer_user);
                }
            }
            if (employee_users != null)
            {
                foreach (var employee_user in employee_users)
                {
                    all_users.Add(employee_user);
                }
            }
            if (User != null)
            {
                var company = new Company();
                if (User.CompanyName != null)
                {
                    company = await _unitofwork.CompanyRepository
                                               .GetFirstOrDefault(
                                                e => e.Name == User.CompanyName ||
                                                e.Name.Contains(User.CompanyName));
                }

                IList<ApplicationUser> supervisor_list = new List<ApplicationUser>();
                IList<ApplicationUser> staff_list = new List<ApplicationUser>();
                IList<ApplicationUser> admin_list = new List<ApplicationUser>();
                IList<ApplicationUser> staff_under_supervisor = new List<ApplicationUser>();

                foreach (var user in all_users)
                {
                    if (user.CompanyId == company.ID ||
                       user.FirstName == User.FirstName ||
                       user.FirstName.Contains(User.FirstName) ||
                       user.LastName == User.LastName ||
                       user.LastName.Contains(User.LastName) ||
                       user.EmployeeStatus == User.EmployeeStatus ||
                       user.PhoneNumber == User.PhoneNumber ||
                       user.PhoneNumber.Contains(User.PhoneNumber))
                    {

                        if (user.EmployeeStatus == SD.Supervisor)
                        {
                            supervisor_list.Add(user);
                        }

                        if (User.StaffUnderSupervisorFirstName != null ||
                           User.StaffUnderSupervisorLastName != null)
                        {
                            if (user.AssignedTo != null && user.EmployeeStatus == SD.Staff)
                            {

                                staff_list.Add(user);
                            }

                            if (staff_list != null && supervisor_list != null)
                            {
                                foreach (var staff_user in staff_list)
                                {
                                    foreach (var supervisor in supervisor_list)
                                    {
                                        if (supervisor.FirstName == User.StaffUnderSupervisorFirstName ||
                                            supervisor.FirstName.Contains(User.StaffUnderSupervisorFirstName) ||
                                            supervisor.LastName == User.StaffUnderSupervisorLastName ||
                                            supervisor.LastName.Contains(User.StaffUnderSupervisorLastName))
                                        {
                                            if (staff_user.AssignedTo == supervisor.Id)
                                            {
                                                staff_under_supervisor.Add(staff_user);
                                            }


                                        }
                                    }
                                }
                                if (staff_under_supervisor.Count != 0)
                                {
                                    return staff_under_supervisor;
                                }
                            }

                            if (User.supervisorsonly)
                            {
                                return supervisor_list;
                            }
                            if (User.staffonly)
                            {
                                return staff_list;
                            }
                            if (User.administratorsonly)
                            {
                                foreach (var adminuser in admin_users)
                                {
                                    if (adminuser.AdministrativeStatus != SD.ChiefAdmin)
                                    {
                                        admin_list.Add(adminuser);
                                    }
                                }
                                return admin_list;
                            }
                            return all_users;
                        }

                    }
                }

            }
            return all_users;
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllCustomerStaff(ApplicationUser User)
        {
            IList<ApplicationUser> filters = new List<ApplicationUser>();
            var customers = await _usermanager.GetUsersInRoleAsync(SD.Customer);

            foreach (var customer in customers)
            {
                if (User.FirstName == customer.FirstName)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.LastName == customer.LastName)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.Email == customer.Email)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.PhysicalAdress == customer.PhysicalAdress)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.DigitalAddress == customer.DigitalAddress)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.City == customer.City)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.Region == customer.Region)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            foreach (var customer in customers)
            {
                if (User.PhoneNumber == customer.PhoneNumber)
                {
                    filters.Add(customer);
                }
                return filters;
            }
            return customers;
        }
        public async Task<string> CreateProductReview(CreateProductReviewQueryFilter filter, ProductReview productreview)
        {
            var all_reviews_for_product = await _unitofwork.ProductReviewRepository.GetAll(e => e.ProductID == filter.productId);
            IList<ProductReview> all_reviews_for_product_list = new List<ProductReview>();
            if (all_reviews_for_product != null)
            {
                foreach (var review in all_reviews_for_product)
                {
                    if (review.UserID == filter.userId)
                    {
                        all_reviews_for_product_list.Add(review);
                    }
                }
            }
            var user = await _usermanager.FindByIdAsync(filter.userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var product = await _unitofwork.ProductRepository.Get(filter.productId);
                if (product != null)
                {
                    if (all_reviews_for_product_list.Count > 0)
                    {
                        throw new BusinessException("You have already written a review on this product, kindly delete it to create another one.");
                    }
                    if (productreview != null)
                    {
                        var product_review = new ProductReview
                        {
                            ProductID = product.Id,
                            UserID = user.Id,
                            Comment = productreview.Comment,
                            Rating = productreview.Rating
                        };
                        product_review.Date = DateTime.Now;
                        await _unitofwork.ProductReviewRepository.Add(product_review);
                        await _unitofwork.Save();
                        return $"Review for {product.Name} created successfully";
                    }
                    throw new BusinessException("Product review cannot be null");

                }
                throw new BusinessException("The product does not exist");
            }
            throw new BusinessException("The user does not exist or is not a customer");
        }
        public async Task<string> DeleteProductReview(CreateProductReviewQueryFilter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.userId);

            var all_product_reviews_for_product = await _unitofwork.ProductReviewRepository.GetAll(e => e.ProductID == filter.productId);

            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (all_product_reviews_for_product != null)
                {
                    foreach (var product in all_product_reviews_for_product)
                    {
                        if (product.UserID == filter.userId)
                        {
                            _unitofwork.ProductReviewRepository.Remove(product.Id);
                            await _unitofwork.Save();
                            var product_review_likes = await _unitofwork.ProductReviewLikeRepository.GetAll(e => e.ProductReviewID == product.Id);
                            foreach (var like_review in product_review_likes)
                            {
                                _unitofwork.ProductReviewLikeRepository.Remove(like_review);
                                await _unitofwork.Save();
                            }
                            return "Your Review has been removed successfully";
                        }
                        throw new BusinessException("Your review is not included in this project");
                    }

                }
                throw new BusinessException("There are no reviews for this product");
            }
            throw new BusinessException("The user does not exist or is not a customer");
        }
        public async Task<string> UpdateProductReview(CreateProductReviewQueryFilter filter, ProductReview productreview)
        {
            var user = await _usermanager.FindByIdAsync(filter.userId);
            var reviews_for_product = await _unitofwork.ProductReviewRepository.GetAll(e => e.ProductID == filter.productId);
            if (reviews_for_product != null)
            {
                if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
                {
                    if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                    {
                        throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                    }
                    foreach (var review in reviews_for_product)
                    {
                        if (review.UserID == filter.userId)
                        {
                            if (productreview != null)
                            {
                                review.Comment = productreview.Comment;
                                review.Rating = productreview.Rating;
                                review.Date_Updated = DateTime.Now;
                                _unitofwork.ProductReviewRepository.Update(review);
                                await _unitofwork.Save();
                            }
                        }
                    }
                }
                throw new BusinessException("The user either does not exist or is not a customer");
            }
            throw new BusinessException("There are no reviews for this product");
        }
        public async Task<PagedList<ProductReview>> GetAllProductReviews(ProductReviewQueryFilter filter)
        {
            IList<ProductReview> product_reviews = new List<ProductReview>();
            var produc = await _usermanager.FindByIdAsync(filter.productId);
            filter.filter.pageSize = filter.filter.pageSize == 0 ? 10 : filter.filter.pageSize;
            filter.filter.pageNumber = filter.filter.pageNumber == 0 ? 1 : filter.filter.pageNumber;      
            var all_reviews_for_product = await _unitofwork.ProductReviewRepository.GetAll(e => e.ProductID == filter.productId);
            if (all_reviews_for_product != null)
            {
                foreach (var product in all_reviews_for_product)
                {
                    if (produc != null)
                    {
                        product_reviews.Add(product);
                    }
                }
                return PagedList<ProductReview>.Create(product_reviews.ToArray(),(int)filter.filter.pageNumber,(int)filter.filter.pageSize);
            }
            return PagedList<ProductReview>.Create(product_reviews, 1, product_reviews.Count); ;
        }
        public async Task<string> ToggleProductReviewLike(string userId, string productreviewId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            var product_review = await _unitofwork.ProductReviewRepository.GetFirstOrDefault(e => e.Id == productreviewId);
            if (product_review == null)
            {
                throw new BusinessException("Product Review Does not exist");
            }
            var all_product_review_likes = await _unitofwork.ProductReviewLikeRepository.GetAll(e => e.ProductReviewID == product_review.Id);
            IList<ProductReviewLike> all_product_review_like_list = new List<ProductReviewLike>(); ;
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                if (all_product_review_likes != null)
                {
                    foreach (var product in all_product_review_likes)
                    {
                        if (product.UserID == user.Id)
                        {
                            _unitofwork.ProductReviewLikeRepository.Remove(product);
                            await _unitofwork.Save();
                            var all = await _unitofwork.ProductReviewLikeRepository.GetAll(e => e.ProductReviewID == product_review.Id);
                            foreach (var single in all)
                            {
                                all_product_review_like_list.Add(single);
                            }

                            return $"{all_product_review_like_list.Count} liked by {user.FirstName} {user.LastName}";
                        }
                    }

                    var productreviewlike = new ProductReviewLike
                    {
                        UserID = user.Id,
                        ProductID = product_review.ProductID,
                        ProductReviewID = product_review.Id
                    };
                    await _unitofwork.ProductReviewLikeRepository.Add(productreviewlike);
                    await _unitofwork.Save();
                    var alls = await _unitofwork.ProductReviewLikeRepository.GetAll(e => e.ProductReviewID == product_review.Id);
                    foreach (var single in alls)
                    {
                        all_product_review_like_list.Add(single);
                    }
                    return $"{all_product_review_like_list.Count} unliked by {user.FirstName} {user.LastName}";
                }
                return $"{all_product_review_like_list.Count}";
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> ToggleStarred(string userId, string companyId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                var stars_of_company = await _unitofwork.StarRepository.GetAll(e => e.CompanyID == companyId);
                var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == companyId);
                if (company == null)
                {
                    throw new BusinessException("This company does not exist");
                }
                IList<Star> star_of_company_list = new List<Star>(); ;
                if (stars_of_company != null)
                {
                    foreach (var star_of_company in stars_of_company)
                    {
                        if (star_of_company.UserID == user.Id)
                        {
                            _unitofwork.StarRepository.Remove(star_of_company.ID);
                            await _unitofwork.Save();
                            var unlike_stars = await _unitofwork.StarRepository.GetAll(e => e.CompanyID == companyId);
                            foreach (var star in unlike_stars)
                            {
                                star_of_company_list.Add(star);
                            }
                            return $"{star_of_company_list.Count} likes unliked by {user.FirstName} {user.LastName}";
                        }

                        var company_like = new Star
                        {
                            UserID = user.Id,
                            CompanyID = company.ID
                        };

                        await _unitofwork.StarRepository.Add(company_like);
                        await _unitofwork.Save();
                        var like_stars = await _unitofwork.StarRepository.GetAll(e => e.CompanyID == companyId);
                        foreach (var star in like_stars)
                        {
                            star_of_company_list.Add(star);
                        }
                        return $"{star_of_company_list.Count} likes liked by {user.FirstName} {user.LastName}";
                    }
                }

                return $"{star_of_company_list.Count}";
            }
            throw new BusinessException("This user is either null or is not a customer");
        }
        public async Task<PagedList<Company>> GetAllStarredCompaniesOfCustomer(StarredCompaniesQueryFIlter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.userId);
            IList<Company> starred_companies_of_user_list = new List<Company>();
            filter.pagination.pageSize = filter.pagination.pageSize == 0 ? 10 : filter.pagination.pageSize;
            filter.pagination.pageNumber = filter.pagination.pageNumber == 0 ? 1 : filter.pagination.pageNumber;
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                var starred_companies_of_user = await _unitofwork.StarRepository.GetAll(e => e.UserID == user.Id);
                foreach (var star in starred_companies_of_user)
                {
                    var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == star.CompanyID);
                    if (company != null)
                    {
                        starred_companies_of_user_list.Add(company);
                    }
                }
                return PagedList<Company>.Create(starred_companies_of_user_list.ToArray(),(int)filter.pagination.pageNumber,(int)filter.pagination.pageSize);
            }
            throw new BusinessException("The user either doesnt exist or its not a customer");
        }
        public async Task<string> CreateShoppingCart(string userId, string productId, int quantity)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var product = await _unitofwork.ProductRepository.GetFirstOrDefault(e => e.Id == productId);
                if (product == null)
                {
                    throw new BusinessException("This product does not exist");
                }
                if(quantity > product.Quantity)
                {
                    throw new BusinessException("The quantity cannot be more than what is in stock");
                }
                var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == userId);
                if (order == null)
                {
                    string[] random_gen = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                    string o_num = "#";
                    foreach(var gen in random_gen)
                    {
                        int index = new Random().Next(random_gen.Length);
                        o_num += random_gen[index];
                        if(o_num.Length == 5)
                        {
                            break;
                        }
                    }
                    var new_order = new Order
                    {
                        UserID = user.Id,
                        OrderNumber = o_num
                    };
                    await _unitofwork.OrderRepository.Add(new_order);
                    await _unitofwork.Save();

                    var cart_order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == user.Id && e.OrderNumber == o_num);
                    if (cart_order == null)
                    {
                        throw new BusinessException("The order for this cart does not exist");
                    }
                    
                    var shopping_cart = new ShoppingCart
                    {
                        orderID = cart_order.Id,
                        ProductID = product.Id,
                        UserID = user.Id,
                        Quantity = quantity,
                        Total = product.Price * quantity


                    };
                    await _unitofwork.ShoppingCartRepository.Add(shopping_cart);
                    await _unitofwork.Save();
                    return $"{product.Name} has been added to your cart";
                }
                var existingorder_shopping_cart = new ShoppingCart
                {
                    orderID = order.Id,
                    ProductID = product.Id,
                    UserID = user.Id,
                    Quantity = quantity,   
                    Total = product.Price * product.Quantity


                };
                await _unitofwork.ShoppingCartRepository.Add(existingorder_shopping_cart);
                await _unitofwork.Save();
                return $"{product.Name} has been added to your cart";
            }
            throw new BusinessException("This user either does not exist or is not a customer");

        }
        public async Task<Order> GoToShoppingCart(string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == userId);
                if (order == null)
                {
                    throw new BusinessException("Your cart is empty");
                }
                return order;
            }
            throw new BusinessException("The user either doesnt exist or is not a customer");
        }
        public async Task<string> DeleteShoppingCart(string userId, string shoppingCartId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            
            if(user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var users_cart = await _unitofwork.ShoppingCartRepository.GetFirstOrDefault(e => e.ID == shoppingCartId);
                var product = await _unitofwork.ProductRepository.GetFirstOrDefault(e => e.Id == users_cart.ProductID);
                if (users_cart != null)
                {
                    if(product == null){
                        throw new BusinessException("This product no longer exists");
                    };  
                    _unitofwork.ShoppingCartRepository.Remove(users_cart.ID);
                    await _unitofwork.Save();
                    return $"{product.Name} has been deleted from your cart successfully";
                }
                throw new BusinessException("Your cart does not exist");
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> UpdateShoppingCart(string userId, string shoppingCartId, int quantity)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var shoppingcart = await _unitofwork.ShoppingCartRepository.Get(shoppingCartId);
                if(shoppingcart != null)
                {
                    var shopping_cart_product = await _unitofwork.ProductRepository.Get(shoppingcart.ProductID);
                    if(shopping_cart_product != null)
                    {
                        if (quantity > shopping_cart_product.Quantity)
                        {
                            throw new BusinessException("The shopping cart for this user is empty");
                        }
                        shoppingcart.Quantity = quantity;
                        shoppingcart.Total = shopping_cart_product.Price * quantity;
                        _unitofwork.ShoppingCartRepository.Update(shoppingcart);
                        await _unitofwork.Save();
                        return "the cart has been updated successfully";
                    }
                    throw new BusinessException("The product for this shopping cart does not exist");
                }
                throw new BusinessException("The shopping cart does not exist");
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> PlaceOrder(string userId, string OrderId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var order = new Order();
                order = await _unitofwork.OrderRepository.Get(OrderId);
                List<ApplicationUser> chief_administrators = new List<ApplicationUser>();
                if (order != null)
                {
                    var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
                    foreach(var admin in administrators)
                    {
                        if(admin.AdministrativeStatus == SD.ChiefAdmin)
                        {
                            chief_administrators.Add(admin);
                        }
                    }
                    int index = new Random().Next(chief_administrators.Count);
                    order.Assignedto = chief_administrators[index].Id;
                    _unitofwork.OrderRepository.Update(order);
                    await _unitofwork.Save();
                    var shopping_carts = await _unitofwork.ShoppingCartRepository.GetAll(e => e.orderID == order.Id && e.UserID == user.Id);
                    int sum = 0;
                    IList<SubAccountEntity> subaccounts = new List<SubAccountEntity>();
                    if (shopping_carts != null)
                    {
                        foreach (var shop in shopping_carts)
                        {
                            sum += (int)shop.Total;
                        }
                        foreach(var shop in shopping_carts)
                        {
                            var product = await _unitofwork.ProductRepository.Get(shop.ProductID);
                            if(product != null)
                            {
                                var company = await _unitofwork.CompanyRepository.Get(product.CompanyID);
                                if(company != null)
                                {
                                    ShoppingCart[] the_cart = new ShoppingCart[] { shop };
                                    var check_subaccount_response = await _paymenthandler.CheckSubAccount(company.Name);
                                    if (check_subaccount_response.Contains("correct"))
                                    {
                                        var existing_subaccount_array = check_subaccount_response.Split("|");
                                        var sub_accountt = new SubAccountEntity
                                        {
                                            subaccount = existing_subaccount_array[0],
                                            share = 18
                                        };
                                        subaccounts.Add(sub_accountt);
                                    }
                                    
                                        var subaccount_code = await _paymenthandler.CreateSubAccount(company.ID, (int)18, the_cart);
                                        var sub_account = new SubAccountEntity
                                        {
                                            subaccount = subaccount_code.Split("|")[1],
                                            share = 18
                                        };
                                        subaccounts.Add(sub_account);      
                                }
                                
                            }
                            
                        }
                        var authorization_url = await _paymenthandler.Handlemultisplitpayments(subaccounts,sum,user.Id);
                        var auth_array = authorization_url.Split("|");
                        order.RefferenceCode = auth_array[1];
                        order.OrderDate = DateTime.Now;
                        order.OrderTotal = sum;                         
                        _unitofwork.OrderRepository.Update(order);
                        await _unitofwork.Save();  
                        return auth_array[0];
                    }

                    throw new BusinessException("The shopping cart does not exist");
                          
                }
                throw new BusinessException("This order does not exist");
                
            }
            throw new BusinessException("The user either doesnt exist or is not a customer");
        }
        public async Task<string> BuyNow(string userId, string productId, int quantity)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var product = await _unitofwork.ProductRepository.Get(productId);
                if(quantity > product.Quantity)
                {
                    throw new BusinessException("The quantity cannot exceed the quantity of the product");
                }
                if(product != null)
                {
                    string[] random_gen = new string[] { "1", "2", "3", "4", "5", "6", "7", "8", "9", "0" };
                    string o_num = "#";
                    foreach (var gen in random_gen)
                    {
                        if (o_num.Length == 5)
                        {
                            break;
                        }
                        int index = new Random().Next(random_gen.Length);
                        o_num += random_gen[index];
                    }
                    var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
                    List<ApplicationUser> chief_admin = new List<ApplicationUser>();
                    foreach (var admin in administrators)
                    {
                        if(admin.AdministrativeStatus == SD.ChiefAdmin)
                        {
                            chief_admin.Add(admin);
                        }
                    }
                    int chief_index = new Random().Next(chief_admin.Count);
                    var order = new Order
                    {
                        UserID = user.Id,
                        OrderNumber = o_num,
                        OrderDate = DateTime.Now,
                        OrderTotal = (int)product.Price * quantity,
                        OrderStatus = SD.AWP,
                        Assignedto = chief_admin[chief_index].Id
                    };
                    await _unitofwork.OrderRepository.Add(order);
                    await _unitofwork.Save();
                    var updated_order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == user.Id && e.OrderNumber == o_num);

                    var shoppingcart = new ShoppingCart
                    {
                        UserID = user.Id,
                        ProductID = product.Id,
                        orderID = updated_order.Id,
                        Quantity = quantity,
                        Total = product.Price * quantity            
                    };

                    await _unitofwork.ShoppingCartRepository.Add(shoppingcart);
                    await _unitofwork.Save();
                    var shopping = await _unitofwork.ShoppingCartRepository
                        .GetFirstOrDefault(e => e.UserID == user.Id &&
                                            e.ProductID == product.Id &&
                                            e.orderID == updated_order.Id);
                    if(shopping != null)
                    {
                        var product_of_cart = await _unitofwork.ProductRepository.Get(shopping.ProductID);
                        if(product_of_cart == null)
                        {
                            throw new BusinessException("the product in the cart must not be empty");

                        }
                        var company = await _unitofwork.CompanyRepository.Get(product_of_cart.CompanyID);
                        if (company == null) throw new BusinessException("The company does not exist");
                        IList<ShoppingCart> shopping_cart_list = new List<ShoppingCart>();
                        shopping_cart_list.Add(shopping);
                        var shopping_cart_array = shopping_cart_list.ToArray();

                        var check_sub_results = await _paymenthandler.CheckSubAccount(company.Name);
                        if (check_sub_results.Contains("correct"))
                        {
                            var check_sub_array = check_sub_results.Split("|");
                            var authorizationn_url = await _paymenthandler
                            .InitializeTransaction(
                                        check_sub_array[0],
                                        shopping.UserID,
                                        (int)product.Price * shopping.Quantity,
                                        SD.Ghana_Cedis);
                            var autho_array = authorizationn_url.Split("|");
                            updated_order.RefferenceCode = autho_array[1];
                            _unitofwork.OrderRepository.Update(updated_order);
                            await _unitofwork.Save();
                            return autho_array[0];
                        }
                        var subaccount_code = await _paymenthandler.CreateSubAccount(company.ID, (int)18, shopping_cart_array);
                        string[] subaccount_array = Array.Empty<string>();
                        if(subaccount_code.Contains("Subaccount created"))
                        {
                            subaccount_array = subaccount_code.Split("|");   
                        }
                        
                        var authorization_url = await _paymenthandler
                            .InitializeTransaction(
                                        subaccount_array[1],
                                        shopping.UserID, 
                                        (int)product.Price * shopping.Quantity,        
                                        SD.Ghana_Cedis);
                        var auth_array = authorization_url.Split("|");
                        updated_order.RefferenceCode = auth_array[1];
                        _unitofwork.OrderRepository.Update(updated_order);
                        await _unitofwork.Save();
                        return auth_array[0];
                    }
                    throw new BusinessException("There is no Cart for this order");
                }
                throw new BusinessException("The product does not exist");
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> VerifyTransactionService(string userId, string refference, bool liketosave)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            IList<CardDetail> card_detail = new List<CardDetail>();
            if (user != null && await _usermanager.IsInRoleAsync(user,SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var result = await _paymenthandler.VerifyTransaction(userId, refference);
                if (result.Contains("success"))
                {
                    var generated_invoice = await _invoice.GenerateReceipt(user.Id, refference);
                    var get_cards = await _unitofwork.CardDetailRepository.GetAll(e => e.UserId == user.Id);
                    var order = await _unitofwork
                        .OrderRepository
                        .GetFirstOrDefault(e => 
                        e.RefferenceCode == refference && 
                        e.UserID == user.Id);
                    order.PaidFor = true;
                    order.OrderStatus = SD.PR;
                    _unitofwork.OrderRepository.Update(order);
                    await _unitofwork.Save();  

                    foreach(var ccc in get_cards)
                    {
                        card_detail.Add(ccc);
                    }
                    if(card_detail.Count > 2)
                    {
                        throw new BusinessException("Sorry but we cannot have more than two saved cards");
                    }
                    foreach(var getcc in get_cards)
                    {
                        if (getcc.RefferenceCode == null)
                        {
                            getcc.RefferenceCode = refference;
                            _unitofwork.CardDetailRepository.Update(getcc);
                            await _unitofwork.Save();
                        }
                    }
                    
                    if (liketosave == true)
                    {
                       var get_card = await _unitofwork.CardDetailRepository.GetFirstOrDefault(e => e.UserId == user.Id && e.RefferenceCode == refference);  
                        get_card.SavedCard = true;
                        _unitofwork.CardDetailRepository.Update(get_card);  
                        await _unitofwork.Save();

                        //i will send them an invoice email here, to tx spare parts when i start generating invoices
                        await _emailsender.SendEmailAsync(user.Email,
                            "Payment Successful",
                            $"<p>Congratulations, your payment has been received and " +     
                            $"we are processing your order with order refference {refference} accordingly as its currently {order.OrderStatus}.</p>" +   
                            $"<p> Please find attached to this email your Payment Receipt: <a href = {generated_invoice}>Payment Receipt</a></p>");

                        await _emailsender.SendEmailAsync("txwebservicesghana@gmail.com",
                            $"Payment Successful for {user.FirstName} {user.LastName}",
                            $"<p>This email is to notify you of payment made by customer {user.FirstName} {user.LastName} with Refference Code: {refference}</p>" +
                            $"Order is currently {order.OrderStatus}" +
                            $"<p> Please find attached to this email his/her Payment Receipt: <a href = {generated_invoice}>Payment Receipt</a></p>");

                        return $"Payment Successful, Card ending with {get_card.last4} has been saved successfully";       

                    }
                    var card_with_refference = await _unitofwork.CardDetailRepository.GetFirstOrDefault(e => e.RefferenceCode == refference);
                    if(card_with_refference != null)
                    {
                        _unitofwork.CardDetailRepository.Remove(card_with_refference.Id);
                        await _unitofwork.Save();
                        return result;    
                    }
                    return result;
                }
                return result;
            }
            throw new BusinessException("This user either does not exist or is not a customer");
        }
        public async Task<string> ChargeSavedCardBuyNow(string userId, string productId, string signature, int quantity)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var product = await _unitofwork.ProductRepository.Get(productId);
                if(product != null)
                {
                    var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == product.CompanyID);
                    if(company == null)
                    {
                        throw new BusinessException("The company does not exist");
                    }
                    var card_detail = await _unitofwork.CardDetailRepository.GetFirstOrDefault(e => e.UserId == user.Id && e.Signature == signature);
                    if(card_detail != null)
                    {
                        var recharge_card = await _paymenthandler.ChargeTransaction(
                                                                user.Id,
                                                                card_detail.AuthorizationCode + "|" + 
                                                                product.Name + "|" 
                                                                + quantity + "|" + 
                                                                product.Price + "|" +
                                                                company.Name + "|" +
                                                                product.Manufacturer + "|" +
                                                                product.Category,
                                                                (int)product.Price * quantity); 
                        if (recharge_card != null)
                            return recharge_card;

                        return recharge_card;
                    }
                    throw new BusinessException("the card does not exist"); 
                }
                throw new BusinessException("The product cannot be empty");
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> ChargeSavedCardOrder(string userId, string orderId, string signature)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.Id == orderId && e.UserID == user.Id);
                if(order != null)
                {
                    var shopping_cart = await _unitofwork.ShoppingCartRepository.GetAll(e => e.orderID == order.Id);
                    if(shopping_cart != null)
                    {
                        var card_detail = await _unitofwork.CardDetailRepository
                            .GetFirstOrDefault(e =>
                            e.UserId == user.Id &&
                            e.Email == user.EmailForCard &&
                            e.Signature == signature);         
                        if(card_detail != null)
                        {
                            int sum = 0;
                            foreach(var cart in shopping_cart)
                            {   
                                sum += (int)cart.Total;
                            }
                            var rechard_card = await _paymenthandler.ChargeTransaction(user.Id, card_detail.AuthorizationCode, sum);
                            if (rechard_card.Contains("Approved"))
                            {
                                order.OrderStatus = SD.PR;
                                _unitofwork.OrderRepository.Update(order);
                                await _unitofwork.Save();
                            }
                            return rechard_card;
                        }
                        throw new BusinessException("The saved card doesnt exist or has been removed.");
                    }
                    throw new BusinessException("The shopping cart is empty.");
                }
                throw new BusinessException("The order does not exist");
            }
            throw new BusinessException("The user either doesnt exist or is not a customer");
        }
        public async Task<PagedList<Product>> GetAllUserPurchases(StarredCompaniesQueryFIlter filter)
        {
            var user = await _usermanager.FindByIdAsync(filter.userId);
            IList<Product> successfully_purchased_products = new List<Product>();
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                var all_user_purchases = await _paymenthandler.RetreiveAllSuccessfulTransactions(user.Id);

                filter.pagination.pageSize = filter.pagination.pageSize == 0 ? 10 : filter.pagination.pageSize;
                filter.pagination.pageNumber = filter.pagination.pageNumber == 0 ? 1 : filter.pagination.pageNumber;
                if (all_user_purchases != null)
                {   
                    foreach(var purchase in all_user_purchases)
                    {
                        var all_user_receipts = await _unitofwork
                            .ReceiptRepository
                            .GetAll(e =>
                        e.CustomerFirstName == (purchase.customer.first_name ?? user.FirstName) &&
                        e.CustomerLastName == (purchase.customer.last_name ?? user.LastName));     

                        foreach(var receipt in all_user_receipts)
                        {
                            var compan = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => receipt.PurchasedItems.Contains(e.Name));
                            if(compan != null)
                            {
                                var products = await _unitofwork.ProductRepository.GetAll(e =>
                                receipt.PurchasedItems.Contains(e.Name) &&
                                receipt.PurchasedItems.Contains(e.Manufacturer) &&
                                e.CompanyID == compan.ID, m => m.OrderBy(f => f.Name));
                                foreach(var pr in products)
                                {
                                    successfully_purchased_products.Add(pr);
                                }
                            }
                            throw new BusinessException("The company does not exist");
                        }
                    }
                    if(successfully_purchased_products != null)
                    {
                        return PagedList<Product>.Create(successfully_purchased_products.ToArray(),(int)filter.pagination.pageNumber,(int)filter.pagination.pageSize);
                    }
                    throw new BusinessException("There are no recently purchased Products");  
                }
                    
                throw new BusinessException("No purchases have been made so far");
            }
            throw new BusinessException("The user either doesn't exist or is not a customer");
        }
        public async Task<string> UploadUserImage(string userId, FileUpload file)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null)
            {
                var image_url = await _imagehandler.CreateImage(user.Id, SD.User_type, file);
                if(image_url != null)
                {
                    
                    await _unitofwork.UserImageRepository.Add(new UserImage
                    {
                        UserId = user.Id,
                        ImageUrl = image_url
                    });
                    await _unitofwork.Save();
                    return $"Your image has been uploaded successfully for {user.FirstName} {user.LastName}";
                }
                throw new BusinessException("The image url does not exist");
            }
            throw new BusinessException("The user does not exist");
        }
        public async Task<string> DeleteUserImage(string userId, string imageId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null)
            {
                var user_image = await _unitofwork.UserImageRepository.GetFirstOrDefault(e => e.UserId == user.Id && e.Id == imageId);
                
                if (user_image != null)
                {
                    _unitofwork.UserImageRepository.Remove(user_image.Id);
                    await _unitofwork.Save();
                }
                var user_image_all = await _unitofwork.UserImageRepository.GetAll(e => e.UserId == user.Id);
                IList<UserImage> user_list = new List<UserImage>();

                foreach (var image_user in user_image_all)
                {
                    user_list.Add(image_user);
                }
                if(user_list.Count == 0)
                {
                    await _unitofwork.UserImageRepository.Add(new UserImage
                    {
                        UserId = user.Id,
                        ImageUrl = "<A DEFAULT IMAGE FROM BUCKET>"
                    });
                }
            }
            throw new BusinessException("The user does not exist");
        }   

    }
}
