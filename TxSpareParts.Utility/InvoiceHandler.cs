using Gehtsoft.PDFFlow.Builder;
using Gehtsoft.PDFFlow.Models.Enumerations;
using Gehtsoft.PDFFlow.Models.Shared;
using Gehtsoft.PDFFlow.Utils;
using Gehtsoft.PDFFlow.UserUtils;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Core.Entities;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Firebase.Auth;
using System.Threading;
using Firebase.Storage;
using System.Threading.Tasks;
using TxSpareParts.Core.Exceptions;
using System;
using TxSpareParts.Utility.interfaces;

namespace TxSpareParts.Utility
{
    public class InvoiceHandler : IInvoiceHandler
    {
        private readonly IUnitOfWork _unitofwork;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IWebHostEnvironment _hostenvironment;

        public InvoiceHandler(
            IUnitOfWork unitofwork,
            UserManager<ApplicationUser> usermanager,
            IWebHostEnvironment hostenvironment)
        {
            _unitofwork = unitofwork;
            _usermanager = usermanager;
            _hostenvironment = hostenvironment;  
        }
        public async Task<string> GenerateReceipt(string userId, string refferencecode)     
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user != null)
            {
                var firebase = new FirebaseAuthProvider(new FirebaseConfig("apikey"));
                var sign_in = await firebase.SignInWithEmailAndPasswordAsync("email", "password");
                var cancellation = new CancellationTokenSource();

                var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == user.Id && e.RefferenceCode == refferencecode);
                var receipt = await _unitofwork.ReceiptRepository.GetFirstOrDefault(e => e.UserId == user.Id && e.ReferenceCode == refferencecode);
                var shoppingcarts = await _unitofwork.ShoppingCartRepository.GetAll(e => e.UserID == user.Id && e.orderID == order.Id);

                var shoppingcarts_list = shoppingcarts.ToList();
                IList<ProductReceipt> product_receipt = new List<ProductReceipt>();
                int sum = 0;
                foreach(var shop in shoppingcarts)
                {
                    var product = await _unitofwork.ProductRepository.Get(shop.ProductID);
                    if(product != null)
                    {
                        var company = await _unitofwork.CompanyRepository.Get(product.CompanyID);
                        if(company != null)
                        {
                            var product_image = await _unitofwork.ProductImageRepository.GetFirstOrDefault(e => e.ProductId == product.Id);
                            if(product_image != null)
                            {
                                var product_receipt_instance = new ProductReceipt
                                {
                                    CompanyName = company.Name,
                                    ProductName = product.Name,
                                    Manufacturer = product.Manufacturer ?? "Not Provided",
                                    Price = product.Price,
                                    Quantity = shop.Quantity,
                                    Total = (int)product.Price * shop.Quantity
                                };
                                sum += (int)product.Price * shop.Quantity;          
                                var image = ImageBuilder.New();
                                image.SetFile(product_image.ImageUrl);
                                image.SetScale(ScalingMode.UserDefined);
                                image.SetSize(14, 14);
                                image.SetMargins(4);
                                product_receipt_instance.ProductImage = image;   
                                product_receipt.Add(product_receipt_instance);
                            }          
                        }
                    }
                }

                
                var url = $"{_hostenvironment.WebRootPath }/Invoices/{ user.Id}/{ receipt.OrderNumber}";
                if (order.OrderStatus == SD.SHI)
                {
                    url = $"{_hostenvironment.WebRootPath }/Receipts/{ user.Id}/{ receipt.OrderNumber}";
                }
                if (order.OrderStatus == SD.CAN)
                {
                    url = $"{_hostenvironment.WebRootPath }/Cancellations/{ user.Id}/{ receipt.OrderNumber}";   
                }

                var upload_url = string.Empty;  
                using (var mystream = new FileStream($"{url}/{receipt.ReferenceCode}.pdf", FileMode.Create))   
                {

                    var document = DocumentBuilder.New();
                    document.AddSectionToDocument(section =>
                    {
                        section.SetOrientation(PageOrientation.Portrait);
                        var order_section = section.AddParagraph($"Order: {receipt.OrderNumber}");
                        order_section.SetAlignment(HorizontalAlignment.Left);
                        order_section.SetMargins(20, 20, 0, 0);

                        var image = section.AddImage("/image/gg.jpeg", new XSize(120, 120), ScalingMode.UserDefined);
                        image.SetAlignment(HorizontalAlignment.Left);
                        image.SetMargins(20, 40, 0, 0);
                        

                        var description = section.AddParagraph(
                            $"Name of Customer: {receipt.CustomerFirstName} {receipt.CustomerLastName} \n" +
                            $"Phone #: {receipt.CustomerPhoneNumber} \n" +
                            $"Digital Address: {user.DigitalAddress ?? "Not Provided"} \n" +
                            $"Physical Address: {user.PhysicalAdress} \n" +
                            $"Email Address: {user.Email} \n");

                        description.SetMargins(0, 40, 20, 0);
                        description.SetAlignment(HorizontalAlignment.Right);

                        var division_line = section.AddLine();
                        division_line.SetWidth(1.5f);
                        division_line.SetMarginTop(60);
                        division_line.SetAlignment(HorizontalAlignment.Center);
                        division_line.SetColor(Color.Black);
                        division_line.SetWidth(2.0f);

                        var table_section = section.AddTable();
                        table_section.SetDataSource(product_receipt.AsEnumerable());
                        table_section.SetAlignment(HorizontalAlignment.Center);
                        table_section.SetMarginTop(65);
                        table_section.AddRow()
                            .SetBackColor(Color.FromRgba(0, 0.69, 0.94, 1))
                            .SetBold()
                            .AddCellToRow("Total")
                            .AddCell($"{sum}")
                            .SetHorizontalAlignment(HorizontalAlignment.Left)
                            .SetColSpan(product_receipt.ToArray().Length - 1);

                        var card_section = section.AddParagraph($"Card Details \n " +
                            $"Bin: {receipt.Bin} \n " +
                            $"Bank: {receipt.Bank} \n " +
                            $"Payment Status: {receipt.GatewayResponse} \n" +
                            $"Order Status: {order.OrderStatus}" +
                            $"Card Type: {receipt.CardType} \n" +
                            $"Last 4: {receipt.last4} \n" +
                            $"Exp.Month / Exp.Year: {receipt.ExpMonth}/{receipt.ExpYear} \n" +
                            $"Channel:{receipt.Channel} \n" +
                            $"Refference Code: {receipt.ReferenceCode ?? order.RefferenceCode}" +
                            $"Estimated Delivery Date: {order.EstimatedDeliveryDate ?? "Not Provided"}" +      
                            $"Transaction Date: {receipt.TransactionDate} \n" );

                        var footer = section.AddFooterToBothPagesToSection(2.0f);
                        footer.AddParagraph("TX Spare Parts \n" +
                            "Digital Address: GA-099-1889 \n" +
                            "Location: 13 Florencia Drive \n" +
                            "Phone Number: +233547889872");
                        footer.SetMargins(50, 50, 50, 50);


                    });
                    
                    document
                    .Build(mystream);   

                    var upload = new FirebaseStorage("<STORAGE BUCKET>", new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(sign_in.FirebaseToken),
                        ThrowOnCancel = true
                    })
                                .Child(url.Split("/")[1])
                                .Child(url.Split("/")[2])
                                .Child(url.Split("/")[3])
                                .Child(url.Split("/")[4])
                                .PutAsync(mystream, cancellation.Token);      
                    mystream.Close();
                    return upload.TargetUrl;
                }
                

            }
            throw new BusinessException("The user does not exist");
        }

        
    }
}
