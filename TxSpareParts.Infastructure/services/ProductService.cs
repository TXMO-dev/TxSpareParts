using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Infastructure.DTO;
using TxSpareParts.Infastructure.Interfaces;
using TxSpareParts.Utility;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Infastructure.services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IEmailSender _emailsender;
        private readonly IImageHandler _imagehandler;

        public ProductService(
            IUnitOfWork unitofwork,
            UserManager<ApplicationUser> usermanager,
            IEmailSender emailsender,
            IImageHandler imagehandler)
        {
            _unitofwork = unitofwork;
            _usermanager = usermanager;
            _emailsender = emailsender;
            _imagehandler = imagehandler;    
        }

        public async Task<string> CreateProduct(string id, Product product)
        {
            var logged_in_user = await _usermanager.FindByIdAsync(id);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == logged_in_user.CompanyId);
            var products = await _unitofwork.ProductRepository.GetAll(product => product.CompanyID == company.ID);
            var employees = await _usermanager.GetUsersInRoleAsync(SD.Employee);
            var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);

            IList<ApplicationUser> company_administrators = new List<ApplicationUser>(); ;
            IList<ApplicationUser> staffs = new List<ApplicationUser>(); ;
            IList<ApplicationUser> supervisors = new List<ApplicationUser>(); ;
            IList<ApplicationUser> staff_of_supervisor = new List<ApplicationUser>(); ;
            IList<Product> company_products = new List<Product>(); ;

            if (company != null)
            {
                if (products != null)
                {
                    foreach (var produce in products)
                    {
                        if (produce.CompanyID == company.ID)
                        {
                            company_products.Add(produce);
                        }
                    }
                }
                foreach (var employee in employees)
                {
                    if (employee.EmployeeStatus == SD.Staff)
                    {
                        staffs.Add(employee);
                    }
                    if (employee.EmployeeStatus == SD.Staff && employee.CompanyId == company.ID && employee.AssignedTo == logged_in_user.Id)
                    {
                        staff_of_supervisor.Add(employee);
                    }
                    supervisors.Add(employee);
                }

                foreach (var administrator in administrators)
                {
                    if (administrator.CompanyId == company.ID)
                    {
                        company_administrators.Add(administrator);
                    }
                }



                foreach (var companyadmin in company_administrators)
                {
                    foreach (var supervisor in supervisors)
                    {
                        foreach (var staff in staffs)
                        {
                            if (staff.Id == logged_in_user.Id && products != null)
                            {
                                foreach (var prod in products)
                                {
                                    if (prod.Name == product.Name || prod.Name.Contains(product.Name) && prod.CompanyID == logged_in_user.CompanyId)
                                    {
                                        prod.Quantity += product.Quantity;
                                        prod.Category = product.Category;
                                        prod.NumberOfUpdates += 1;
                                        prod.recently_updated_by = logged_in_user.Id;
                                        _unitofwork.ProductRepository.Update(prod);
                                        await _unitofwork.Save();
                                        return $"the product named {prod.Name} already exists and has been updated successfully by " +
                                            $"{logged_in_user.FirstName} {logged_in_user.LastName}";
                                    }
                                }
                            }
                            if ((staff.Id == logged_in_user.Id && staff.CompanyId == logged_in_user.CompanyId) ||
                                (supervisor.Id == logged_in_user.Id && supervisor.CompanyId == logged_in_user.CompanyId) ||
                                companyadmin.Id == logged_in_user.Id
                                )
                            {
                                product.userID = logged_in_user.Id;
                                product.CompanyID = company.ID;
                                product.recently_updated_by = logged_in_user.Id;
                                product.NumberOfUpdates += 1;
                                if(product.Category != null)
                                {
                                    if (product.Category == SD.OEM)
                                    {
                                        product.Category = SD.OEM;
                                    }
                                    if (product.Category == SD.GOE)
                                    {
                                        product.Category = SD.GOE;
                                    }
                                    if (product.Category == SD.AP)
                                    {
                                        product.Category = SD.AP;
                                    }
                                    if (product.Category == SD.RP)
                                    {
                                        product.Category = SD.RP;
                                    }
                                    if (product.Category == SD.RCP)
                                    {
                                        product.Category = SD.RCP;
                                    }
                                    if (product.Category == SD.SP)
                                    {
                                        product.Category = SD.SP;
                                    }
                                }
                                await _unitofwork.ProductRepository.Add(product);
                                await _unitofwork.Save();
                                if (supervisor.Id == logged_in_user.Id && supervisor.CompanyId == logged_in_user.CompanyId)
                                {
                                    company_products.Add(product);
                                    foreach (var company_product in company_products)
                                    {
                                        foreach (var stafff in staff_of_supervisor)
                                        {
                                            int index = new Random().Next(staff_of_supervisor.Count);

                                            if ((stafff.AssignedTo == supervisor.Id && (stafff.FirstName == product.SupervisorName)) ||
                                                (stafff.AssignedTo == supervisor.Id && (stafff.LastName == product.SupervisorName)) ||
                                                (stafff.AssignedTo == supervisor.Id && stafff.FirstName.Contains(product.SupervisorName)) ||
                                                (stafff.AssignedTo == supervisor.Id && stafff.LastName.Contains(product.SupervisorName))
                                                )
                                            {
                                                company_product.userID = stafff.Id;
                                                company_product.NumberOfUpdates += 1;
                                                company_product.from_supervisor = supervisor.Id;
                                                _unitofwork.ProductRepository.Update(company_product);
                                                await _unitofwork.Save();
                                                await _emailsender.SendEmailAsync(stafff.Email,
                                                                                  "Product Assigned",
                                                                                  $"<p>A product has been made by your supervisor " +
                                                                                  $"{supervisor.FirstName} {supervisor.LastName}</p>" +
                                                                                  $"<p> and has been designated to you to manage.");

                                            }

                                            if (stafff.AssignedTo == supervisor.Id &&
                                                    company_product.userID == supervisor.Id)
                                            {

                                                company_product.userID = staff_of_supervisor[index].Id;
                                                company_product.NumberOfUpdates += 1;
                                                company_product.from_supervisor = supervisor.Id;
                                                _unitofwork.ProductRepository.Update(company_product);
                                                await _unitofwork.Save();
                                                await _emailsender.SendEmailAsync(staff_of_supervisor[index].Email,
                                                                                  "Product Assigned",
                                                                                  $"<p>A product has been made by your supervisor " +
                                                                                  $"{supervisor.FirstName} {supervisor.LastName}</p>" +
                                                                                  $"<p> and has been designated to you to manage.");

                                            }
                                        }

                                    }
                                    company_products.Remove(product);
                                }
                            }


                        }
                    }
                }




            }
            throw new BusinessException("The company does not exist");
        }
        public async Task<string> DeleteProduct(string id)
        {
            var administrators = await _usermanager.GetUsersInRoleAsync(SD.Admin);
            var product = await _unitofwork.ProductRepository.Get(id);
            
            if(product == null)
            {
                throw new BusinessException("This product does not exist");
            }
            _unitofwork.ProductRepository.Remove(id);
            var product_images = await _unitofwork.ProductImageRepository.GetAll(e => e.ProductId == product.Id);
            if(product_images != null)
            {
                foreach(var product_image in product_images)
                {
                     _unitofwork.ProductImageRepository.Remove(product_image.Id);
                }
            }
            await _unitofwork.Save();
            var user = await _usermanager.FindByIdAsync(product.userID);
            var all_likes_for_product = await _unitofwork.LikeRepository.GetAll(e => e.productID == product.Id);
            foreach(var product_like in all_likes_for_product)
            {
                _unitofwork.LikeRepository.Remove(product_like.Id);
                await _unitofwork.Save();
            }
            if(user.EmployeeStatus == SD.Staff)
            {
                var supervisor = await _usermanager.FindByIdAsync(user.AssignedTo);
                await _emailsender.SendEmailAsync(supervisor.Email,
                    $"{product.Name} has been deleted",
                    $"A product called {product.Name} has been deleted under your supervision");
                foreach(var administrator in administrators)
                {
                    if(administrator.CompanyId == supervisor.CompanyId)
                    {
                        await _emailsender.SendEmailAsync(administrator.Email,
                        $"{product.Name} has been deleted",
                        $"<p>A product called {product.Name} has been deleted under your administration " +
                        $"from the team of supervisor {supervisor.FirstName} {supervisor.LastName}, " +
                        $"posted or managed by staff {user.FirstName} {user.LastName}</p>");
                    }
                }
            }
            return $"{product.Name} has been deleted successfully successfully";
        }
        public async Task<PagedList<Product>> ReadAndFilterAllProducts(string id, int? pageNumber, int? pageSize, Product product)
        {
            pageNumber = pageNumber == 0 ? 1 : pageNumber;
            pageSize = pageSize == 0 ? 20 : pageSize;
            IList<Product> staff_products = new List<Product>();
            IList<Product> supervisor_products = new List<Product>(); ;
            IList<Product> administrator_products = new List<Product>(); ;
            var all_products = await _unitofwork.ProductRepository.GetAll();
            IList<Product> all_product_list = new List<Product>(); ;
            IList<Product> all_name_product_list = new List<Product>(); ;
            foreach (var pr in all_products)
            {
                all_product_list.Add(pr);
            }
            var user = await _usermanager.FindByIdAsync(id);
            var products = await _unitofwork.ProductRepository.GetAll(product => product.CompanyID == user.CompanyId);
            var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == user.CompanyId);
            if (user.EmployeeStatus == SD.Staff)
            {
                foreach (var produc in products)
                {
                    if (produc.userID == user.Id)
                    {
                        if (product.Name != null)
                        {
                            var products_by_name = await _unitofwork.ProductRepository.GetAll(e => e.Name == product.Name && e.userID == user.Id);
                            if(products_by_name != null)
                            {
                                foreach (var production in products_by_name)
                                {
                                    staff_products.Add(production);
                                }
                            }
                            if(pageNumber != null && pageSize != null)
                            {
                                return PagedList<Product>.Create(staff_products.ToArray(),(int)pageNumber,(int)pageSize);
                            }
                            
                        }
                        staff_products.Add(produc);
                    }
                }
 
                return PagedList<Product>.Create(staff_products.ToArray(), 1, staff_products.Count()); ;
            }
            if (user.EmployeeStatus == SD.Supervisor)
            {
                foreach (var prod in products)
                {
                    var prod_user = await _usermanager.FindByIdAsync(prod.userID);
                    if (prod_user != null) {
                        if (prod_user.EmployeeStatus == SD.Staff && prod_user.AssignedTo == user.Id)
                        {
                            if(product.Name != null)
                            {
                                var product_by_name = await _unitofwork.ProductRepository.GetAll(e => e.Name == product.Name && product.CompanyID == user.CompanyId);
                                foreach(var produ in product_by_name)
                                {
                                    var produ_user = await _usermanager.FindByIdAsync(produ.userID);
                                    if(produ_user.EmployeeStatus == SD.Staff && produ_user.AssignedTo == user.Id)
                                    {
                                        supervisor_products.Add(produ);
                                    }
                                }
                                if(pageNumber != null && pageSize != null)
                                {
                                    return PagedList<Product>.Create(supervisor_products.ToArray(), (int)pageNumber, (int)pageSize);
                                }
                            }
                            supervisor_products.Add(prod);
                        }
                    }

                }

                return PagedList<Product>.Create(supervisor_products.ToArray(), 1, supervisor_products.Count()); 
            }

            if (user.AdministrativeStatus != SD.ChiefAdmin && await _usermanager.IsInRoleAsync(user, SD.Admin))
            {
                foreach (var pro in products)
                {
                    if(product.Name != null)
                    {
                        var product_by_name = await _unitofwork.ProductRepository.GetAll(e => e.Name == product.Name && e.CompanyID == user.CompanyId);
                        foreach(var p in product_by_name)
                        {
                            administrator_products.Add(p);
                        }
                        if(pageNumber != null && pageSize != null)
                        {
                            return PagedList<Product>.Create(administrator_products.ToArray(), (int)pageNumber, (int)pageSize);
                        }
                        
                    }
                    administrator_products.Add(pro);
                }
                return PagedList<Product>.Create(administrator_products.ToArray(), 1, administrator_products.Count()); ;
            }
            if(product.Name != null)
            {
                var all_product_list_by_name = await _unitofwork.ProductRepository.GetAll(e => e.Name == product.Name);
                foreach(var prod in all_product_list_by_name)
                {
                    all_name_product_list.Add(prod);
                }
                if(pageNumber != null && pageSize != null)
                {
                    return PagedList<Product>.Create(all_name_product_list.ToArray(), (int)pageNumber, (int)pageSize);
                }

            }
            
            return PagedList<Product>.Create(all_name_product_list.ToArray(), 1, all_name_product_list.Count()); 

        } 
        public async Task<Product> ReadProduct(string id)
        {
            var product = await _unitofwork.ProductRepository.Get(id);
            return product;
        }
        public async Task<string> UpdateProduct(string id, string productId, Product product)
        {
            var logged_in_user = await _usermanager.FindByIdAsync(id);
            if(logged_in_user != null)
            {
                var company = _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == logged_in_user.CompanyId);
                if(company != null)
                {
                    if (logged_in_user.EmployeeStatus == SD.Staff)
                    {
                        var produce = await _unitofwork.ProductRepository.Get(productId);
                        if (produce != null && produce.CompanyID == logged_in_user.CompanyId && produce.userID == logged_in_user.Id)
                        {
                            produce = product;
                            _unitofwork.ProductRepository.Update(produce);
                            await _unitofwork.Save();
                        }
                    }

                    if(logged_in_user.EmployeeStatus == SD.Supervisor)
                    {
                        var produce = await _unitofwork.ProductRepository.Get(productId);
                        if (produce != null && produce.CompanyID == logged_in_user.CompanyId)
                        {
                            var user = await _usermanager.FindByIdAsync(produce.userID);
                            var updated_user = await _usermanager.FindByIdAsync(produce.recently_updated_by);
                            if(user.AssignedTo == logged_in_user.Id || updated_user.AssignedTo == logged_in_user.Id)
                            {
                                produce = product;
                                _unitofwork.ProductRepository.Update(produce);
                                await _unitofwork.Save();
                            }
                        }
                    }

                    if(await _usermanager.IsInRoleAsync(logged_in_user, SD.Admin))
                    {
                        if(logged_in_user.AdministrativeStatus != SD.ChiefAdmin)
                        {
                            var produce = await _unitofwork.ProductRepository.Get(productId);
                            if (produce != null && produce.CompanyID == logged_in_user.CompanyId)
                            {
                                produce = product;
                                _unitofwork.ProductRepository.Update(produce);
                                await _unitofwork.Save();
                            }
                        }        
                    }

                    if(logged_in_user.AdministrativeStatus == SD.ChiefAdmin)
                    {
                        var produce = await _unitofwork.ProductRepository.Get(productId);
                        produce = product;
                        _unitofwork.ProductRepository.Update(produce);
                        await _unitofwork.Save();
                    }
                    return "The product has been updated successfully";
                }
                throw new BusinessException("The company does not exist");

            }
            throw new BusinessException("The user does not exist");
        }
        public async Task<string> UploadProductImage(string productId, FileUpload file)
        {
            var product = await _unitofwork.ProductRepository.Get(productId);
            if (product != null)
            {
                var all_products_images = await _unitofwork.ProductImageRepository.GetAll(e => e.ProductId == productId);
                var all_product_list = all_products_images.ToList();
                if (all_product_list.Count == 5)
                {
                    throw new BusinessException("The maximum image of image uploads is 5. Kindly delete an image to upload a new one");
                }
                var product_image_url = await _imagehandler.CreateImage(product.Id, SD.Product_type, file);
                if (product_image_url != null)
                {
                    await _unitofwork.ProductImageRepository.Add(new ProductImage
                    {
                        ProductId = product.Id,
                        ImageUrl = product_image_url
                    });
                    await _unitofwork.Save();
                    return $"Your image has been uploaded successfully for {product.Name}";
                }
                throw new BusinessException("The product image url does not exist");
            }
            throw new BusinessException("The product does not exist");
        }
        public async Task<string> DeleteProductImage(string productId, string productImageId)
        {
            var product = await _unitofwork.ProductRepository.Get(productId);
            if (product != null)
            {
                var all_products_images = await _unitofwork.ProductImageRepository.GetAll(e => e.ProductId == product.Id);
                var all_product_list = all_products_images.ToList();
                if (all_product_list.Count == 1)
                {
                    throw new BusinessException("The product image cannot be deleted if its only one, " +
                        "upload a new image and delete this existing one");
                }
                var product_image = await _unitofwork.ProductImageRepository.Get(productImageId);
                if (product_image != null)
                {
                    _unitofwork.ProductImageRepository.Remove(product_image.Id);
                    await _unitofwork.Save();
                    return $"Product image of {product.Name} has been deleted successfully";
                }
                throw new BusinessException("The product image could not be deleted successfully");
            }
            throw new BusinessException("The product does not exist");
        }
    }
}
