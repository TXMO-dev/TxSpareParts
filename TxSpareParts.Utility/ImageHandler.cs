using Firebase.Auth;
using Firebase.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Utility
{
    public class ImageHandler : IImageHandler       
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IWebHostEnvironment _environment;   

        public ImageHandler(
            IUnitOfWork unitofwork,
            UserManager<ApplicationUser> usermanager,
            IWebHostEnvironment environment
            )
        {
            _unitofwork = unitofwork;
            _usermanager = usermanager;
            _environment = environment;
        }
        public async Task<string> CreateImage(string Id,string type, FileUpload file)
        {
            var user = new ApplicationUser();
            var product = new Product();
            var company = new Company();

            var firebase = new FirebaseAuthProvider(new FirebaseConfig("apikey"));
            var sign_in = await firebase.SignInWithEmailAndPasswordAsync("email", "password");    
            var cancellation = new CancellationTokenSource();

            if (type == SD.User_type)
            {
                user = await _usermanager.FindByIdAsync(Id);
            }
            if(type == SD.Product_type)
            {
                product = await _unitofwork.ProductRepository.Get(Id);
            }
            if(type == SD.Company_type)
            {
                company = await _unitofwork.CompanyRepository.Get(Id);
            }
            
            string webrootpath = _environment.WebRootPath;
            var folder = string.Empty;
            
            if (user != null || product != null || company != null)
            {
                if(type == SD.Product_type || type == SD.Company_type || type == SD.User_type)
                {
                    

                    if(file.files.Length > 0)
                    {
                        if(
                            !file.files.FileName.Contains("jpeg") || 
                            !file.files.FileName.Contains("jpg") || 
                            !file.files.FileName.Contains("png"))
                        {
                            throw new BusinessException("The file extension is not supported for image uploads");
                        }
                        if(type == SD.User_type)
                        {
                            folder = $"{type}/{user.Id}/";
                            folder += Guid.NewGuid().ToString();    
                        }
                        if(type == SD.Product_type)
                        {
                            folder = $"{type}/{product.Id}/";
                            folder += Guid.NewGuid().ToString();
                        }
                        if(type == SD.Company_type)
                        {
                            folder = $"{type}/{company.ID}/";
                            folder += Guid.NewGuid().ToString();
                        }
                        

                        var uploads = Path.Combine(webrootpath, folder);
                        if (!Directory.Exists(uploads))
                        {
                            Directory.CreateDirectory(uploads);
                            try
                            {
                                using (FileStream filestream = File.Open(uploads, FileMode.Create, FileAccess.ReadWrite))
                                {

                                    await file.files.CopyToAsync(filestream);
                                    await filestream.FlushAsync();
                                    //return uploads + file.files.FileName;
                                    var upload = new FirebaseStorage("<STORAGE BUCKET>", new FirebaseStorageOptions
                                    {
                                        AuthTokenAsyncFactory = () => Task.FromResult(sign_in.FirebaseToken),
                                        ThrowOnCancel = true
                                    })
                                    .Child(folder.Split("/")[0])
                                    .Child(folder.Split("/")[1])
                                    .Child(folder.Split("/")[2])
                                    .PutAsync(filestream, cancellation.Token);
                                    return upload.TargetUrl;
                                }
                            }catch(UnauthorizedAccessException ex)
                            {
                                throw new BusinessException($"You are not authorized to perform this operation: \n {ex.Message}");      
                            }
                            
                        }

                        try
                        {
                            using (FileStream filestream = File.Open(uploads, FileMode.CreateNew, FileAccess.ReadWrite))
                            {

                                await file.files.CopyToAsync(filestream);
                                await filestream.FlushAsync();
                                //return uploads + file.files.FileName;
                                var upload = new FirebaseStorage("<STORAGE BUCKET>", new FirebaseStorageOptions
                                {
                                    AuthTokenAsyncFactory = () => Task.FromResult(sign_in.FirebaseToken),
                                    ThrowOnCancel = true
                                })  
                                .Child(folder.Split("/")[0])
                                .Child(folder.Split("/")[1])
                                .Child(folder.Split("/")[2])
                                .PutAsync(filestream, cancellation.Token);

                                return upload.TargetUrl;
                            }
                        }catch(IOException ex)
                        {
                            throw new BusinessException($"This file already exists: \n {ex.Message}");
                        }

                            


                        
                        

                    }
                    throw new BusinessException("Upload was unsuccessful");
                }
                throw new BusinessException("The chosen type is not supported");
            }
            throw new BusinessException("The user does not exist");
        }
        
    }
}
