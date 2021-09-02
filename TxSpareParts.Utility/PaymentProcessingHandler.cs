using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using PayStackDotNetSDK.Methods.Transactions;
using PayStackDotNetSDK.Models.SubAccounts;
using PayStackDotNetSDK.Methods.Subaccounts;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.Exceptions;
using TxSpareParts.Core.Interfaces;
using TxSpareParts.Utility.Interfaces;
using PayStackDotNetSDK.Models.Transactions;
using PayStackDotNetSDK.Methods.Banks;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using System.Linq;
using System.Net.Http.Json;
using Newtonsoft.Json;
using TxSpareParts.Utility.interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace TxSpareParts.Utility
{
    public class PaymentProcessingHandler : IPaymentProcessingHandler
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly IUnitOfWork _unitofwork;
        private readonly IHttpClientFactory _clientfactory;
        private readonly IEmailSender _emailhandler;
        private readonly IInvoiceHandler _invoice;
        private readonly IWebHostEnvironment _hostenvironment;

        public PaymentProcessingHandler(
            IConfiguration configuration,
            UserManager<ApplicationUser> usermanager,
            IUnitOfWork unitofwork,
            IHttpClientFactory clientfactory,
            IEmailSender emailhandler,
            IInvoiceHandler invoice,
            IWebHostEnvironment _hostenvironment)
        {
            _configuration = configuration;
            _usermanager = usermanager;
            _unitofwork = unitofwork;
            _clientfactory = clientfactory;
            _emailhandler = emailhandler;
            _invoice = invoice;
        }
        public async Task<string> InitializeTransaction(string subaccount_code, string userId, int amount, string currency)
        {
            var transaction_reference = Guid.NewGuid().ToString();
            var user = await _usermanager.FindByIdAsync(userId);
            if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
            {
                throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
            }
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (user.EmailForCard == null && await _usermanager.IsEmailConfirmedAsync(user))
                {
                    user.EmailForCard = user.Email;
                    await _usermanager.UpdateAsync(user);
                }
                var connectionInstance = new PaystackTransaction(_configuration["Test_Paystack_api_key"]);
                var response = await connectionInstance.InitializeTransaction(new TransactionRequestModel()
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    amount = amount * 100,
                    currency = currency,
                    email = user.EmailForCard,
                    reference = transaction_reference,
                    makeReferenceUnique = true,
                    subaccount = subaccount_code,
                    channels = new string[] { "card", "mobile_money" },             
                    callback_url = $"{_configuration["AppUrl"]}/api/Customer/Customer/verifytransaction?userid={user.Id}&refferencode={transaction_reference}"
                });
                if (response.status)
                {
                    return response.data.authorization_url + "|" + response.data.reference;
                }
                throw new BusinessException($"{response.message}");
            }
            throw new BusinessException("User either doesnt exist or is not a customer");
        }
        public async Task<string> VerifyTransaction(string userId, string refference)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var connectionInstance = new PaystackTransaction(_configuration["Test_Paystack_api_key"]);
                var response = await connectionInstance.VerifyTransaction(refference);
                if (response.status)
                {
                    var transaction_history = response.data.log.history;
                    foreach (var history in transaction_history)
                    {
                        if (history.type == "success")
                        {

                            if (response.data.authorization.reusable == true)
                            {
                                var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == user.Id);
                                var product_names = string.Empty;
                                var company = new Company();
                                if (order != null)
                                {
                                    int sumItems = 0;
                                    int sumTotal = 0;
                                    var shopping_carts = await _unitofwork.ShoppingCartRepository.GetAll(e => e.orderID == order.Id && e.UserID == user.Id);
                                    foreach (var cart in shopping_carts)
                                    {
                                        sumItems += cart.Quantity;
                                        int counter = 0;
                                        var product = await _unitofwork.ProductRepository.Get(cart.ProductID);
                                        company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.ID == product.CompanyID);
                                        if (product != null)
                                        {
                                            sumTotal += (int)product.Price * cart.Quantity;
                                            product_names +=
                                                $" {++counter}. " + "Product Category :" +
                                                product.Category + " | " +
                                                "Product Manufacturer :" + 
                                                product.Manufacturer +   
                                                $"Product Name :" +
                                                product.Name + " | " + " Quantity:" +
                                                product.Quantity + " | " + " Price: " +
                                                product.Price + " | " + " Company Name: " +
                                                company.Name +        
                                                "\n\n";
                                            _unitofwork.ProductRepository.Update(product);
                                            await _unitofwork.Save();
                                        }
                                    }
                                    

                                    await _unitofwork.CardDetailRepository.Add(new CardDetail
                                    {
                                        UserId = user.Id,
                                        AccountName = $"{user.FirstName} { user.LastName}",
                                        Email = user.EmailForCard,
                                        AuthorizationCode = response.data.authorization.authorization_code,
                                        CardType = response.data.authorization.card_type,
                                        last4 = response.data.authorization.last4,
                                        ExpMonth = response.data.authorization.exp_month,
                                        ExpYear = response.data.authorization.exp_year,
                                        Bin = response.data.authorization.bin,
                                        Bank = response.data.authorization.bank,
                                        PaymentChannel = response.data.authorization.channel,
                                        Signature = response.data.authorization.signature,
                                        CountryCode = response.data.authorization.country_code,
                                        TransactionDate = response.data.transaction_date,
                                        Reusable = response.data.authorization.reusable
                                    });

                                    await _unitofwork.ReceiptRepository.Add(new Receipt
                                    {
                                        UserId = user.Id,
                                        Amount = response.data.amount,
                                        TransactionDate = response.data.transaction_date,
                                        Currency = response.data.currency,
                                        ReferenceCode = response.data.reference,
                                        GatewayResponse = response.data.gateway_response,
                                        Channel = response.data.channel,
                                        CustomerFirstName = response.data.customer.first_name ?? user.FirstName,
                                        CustomerLastName = response.data.customer.last_name ?? user.LastName,
                                        CustomerEmail = response.data.customer.last_name ?? user.LastName,
                                        CardType = response.data.authorization.card_type,
                                        last4 = response.data.authorization.last4,
                                        ExpMonth = response.data.authorization.exp_month,
                                        ExpYear = response.data.authorization.exp_year,
                                        Bin = response.data.authorization.bin,
                                        Bank = response.data.authorization.bank,
                                        CountryCode = response.data.authorization.country_code,
                                        CustomerPhoneNumber = user.PhoneNumber,
                                        NumberOfItems = sumItems,
                                        PurchasedItems = product_names,
                                        Total = sumTotal,
                                        OrderNumber = order.OrderNumber
                                    });
                                    await _unitofwork.Save();
                                }

                                return history.message + " " + $"for {response.data.reference}" + " transaction " + response.data.gateway_response;
                            }

                        }
                        if (history.type == "action")
                        {
                            return history.message + " " + $"for {response.data.reference}" + " transaction  " + response.data.gateway_response;
                        }
                        if (history.type == "close")
                        {
                            return history.message + " " + $"for {response.data.reference}" + " transaction " + response.data.gateway_response;
                        }
                    }
                    return response.message + " : " + response.data.gateway_response + " transaction " + response.data.gateway_response; ;
                }
                return response.message + " : " + response.data.gateway_response + " transaction " + response.data.gateway_response; ;
            }
            throw new BusinessException("The user either does not exist or is not a customer");
        }
        public async Task<string> ChargeTransaction(string userId, string authorization_code, int amount)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
            {
                throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
            }
            if (user == null && !await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                throw new BusinessException("This user either does not exist or is not a customer");
            }
            IList<object> Cards_with_same_obj = new List<object>();
            var authorization_code_split = new string[] { };
            var auth_code = string.Empty;
            var product_name = string.Empty;
            var product_quantity = string.Empty;
            var product_price = string.Empty;
            var product_manufacturer = string.Empty;
            var company_name = string.Empty;
            var product_category = string.Empty;
            var connectionInstance = new PaystackTransaction(_configuration["Test_Paystack_api_key"]);
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
            if (authorization_code.Contains("|"))
            {
                authorization_code_split = authorization_code.Split("|");
                auth_code = authorization_code_split[0];
                product_name = authorization_code_split[1];
                product_quantity = authorization_code_split[2];
                product_price = authorization_code_split[3];
                company_name = authorization_code_split[4];
                product_manufacturer = authorization_code_split[5];
                product_category = authorization_code_split[6];

                var the_response = await connectionInstance.ChargeAuthorization(auth_code, user.EmailForCard, amount);
                if (the_response.status)
                {
                    if (the_response.data.authorization.reusable)
                    {
                        var the_cards = await _unitofwork.CardDetailRepository.GetAll(e => e.Email == user.EmailForCard);
                        if (the_cards != null)
                        {
                            foreach (var cc in the_cards)
                            {

                                Cards_with_same_obj.Add(cc);

                                if (cc.Signature == the_response.data.authorization.signature)
                                {
                                    cc.AuthorizationCode = the_response.data.authorization.authorization_code;
                                    cc.RefferenceCode = the_response.data.reference;
                                    cc.TransactionDate = the_response.data.transaction_date;
                                    _unitofwork.CardDetailRepository.Update(cc);

                                    await _unitofwork.ReceiptRepository.Add(new Receipt
                                    {
                                        UserId = user.Id,
                                        Amount = the_response.data.amount,
                                        TransactionDate = the_response.data.transaction_date,
                                        Currency = the_response.data.currency,
                                        ReferenceCode = the_response.data.reference,
                                        GatewayResponse = the_response.data.gateway_response,
                                        Channel = the_response.data.channel,
                                        CustomerFirstName = the_response.data.customer.first_name ?? user.FirstName,
                                        CustomerLastName = the_response.data.customer.last_name ?? user.LastName,
                                        CustomerEmail = the_response.data.customer.last_name ?? user.LastName,
                                        CardType = the_response.data.authorization.card_type,
                                        last4 = the_response.data.authorization.last4,
                                        ExpMonth = the_response.data.authorization.exp_month,
                                        ExpYear = the_response.data.authorization.exp_year,
                                        Bin = the_response.data.authorization.bin,
                                        Bank = the_response.data.authorization.bank,
                                        CountryCode = the_response.data.authorization.country_code,
                                        CustomerPhoneNumber = user.PhoneNumber,
                                        NumberOfItems = int.Parse(product_quantity),
                                        PurchasedItems = $"Product Category: {product_category} | Product Manufacturer: {product_manufacturer} | Product Name: {product_name} | Quantity: {product_quantity} | Price: {product_price} | Company Name: {company_name}",
                                        Total = amount,
                                        OrderNumber = o_num   
                                    });
                                    //i will send an email of the invoice in pdf format to the customer to the company and to tx spare parts.
                                    var company = await _unitofwork.CompanyRepository.GetFirstOrDefault(e => e.Name == company_name);
                                    var generated_invoice = await _invoice.GenerateReceipt(user.Id, the_response.data.reference);
                                    await _emailhandler.SendEmailAsync(user.Email,
                                            "Payment Successful",
                                            $"<p>Congratulations, your payment has been received and " +
                                            $"we are processing your order with order refference {the_response.data.reference} accordingly as its currently on Payment Received.</p>" +
                                            $"<p> Please find attached to this email your Payment Receipt: <a href = {generated_invoice}>Payment Receipt</a></p>");

                                    await _emailhandler.SendEmailAsync("txwebservicesghana@gmail.com",
                                        $"Payment Successful for {user.FirstName} {user.LastName}",
                                        $"<p>This email is to notify you of payment made by customer {user.FirstName} {user.LastName} with Refference Code: {the_response.data.reference}</p>" +
                                        $"Order is currently on Payment Received." +
                                        $"<p> Please find attached to this email his/her Payment Receipt: <a href = {generated_invoice}>Payment Receipt</a></p>");

                                    if(company != null)
                                    {
                                        await _emailhandler.SendEmailAsync(company.Email,
                                        $"Product Purchase",
                                        $"<p>This email is to notify you of purchase made of {product_quantity} quantity/quantities of {product_name} worth {product_price}</p>" +
                                        $"Amounting to a total of {int.Parse(product_price) * int.Parse(product_quantity)}" +
                                        $"Order is currently on Payment Received." +
                                        $"<p>Your payment rollout will be a week from {DateTime.Now.Date}</a></p>");
                                    }
                                    await _unitofwork.Save();
                                    return the_response.message + " " + " transaction " + the_response.data.gateway_response;        
                                }

                            }
                            if (Cards_with_same_obj.Count > 2)
                            {
                                throw new BusinessException("Sorry the maximum number of saved cards is Two(2)");
                            }

                        }
                    }
                }
                
                
            }
            var cards = await _unitofwork.CardDetailRepository.GetAll(e => e.Email == user.EmailForCard);
            if (cards != null)
            {
                foreach (var cc in cards)
                {
                    Cards_with_same_obj.Add(cc);
                }
                if (Cards_with_same_obj.Count > 2)
                {
                    throw new BusinessException("Sorry the maximum number of saved cards is Two(2)");
                }
                foreach (var card in cards)
                {
                    if (card.Email == user.EmailForCard && card.AuthorizationCode == authorization_code)
                    {
                        var response = await connectionInstance.ChargeAuthorization(card.AuthorizationCode, card.Email, amount);

                        if (response.status)
                        {
                            if (response.data.authorization.reusable)
                            {
                                if (response.data.authorization.signature == card.Signature)
                                {
                                    card.AuthorizationCode = response.data.authorization.authorization_code;
                                    card.RefferenceCode = response.data.reference;
                                    card.TransactionDate = response.data.transaction_date;
                                    _unitofwork.CardDetailRepository.Update(card);
                                    
                                    var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == user.Id);
                                    if(order == null)
                                    {
                                        throw new BusinessException("Thr order cannot be empty");
                                    }
                                    var shopping_carts = await _unitofwork.ShoppingCartRepository.GetAll(e => e.UserID == user.Id && e.orderID == order.Id);
                                    
                                    IList<string> companies = new List<string>();

                                    if (shopping_carts == null)
                                        throw new BusinessException("The shopping cart is empty");
                                    int sumItems = 0;
                                    int sumTotal = 0;
                                    var product_names = string.Empty;
                                    foreach (var shopping in shopping_carts)   
                                    {
                                        var production = await _unitofwork.ProductRepository.Get(shopping.ProductID);   
                                        
                                        int counter = 0;
                                        if (production != null)
                                        {
                                            var company = await _unitofwork.CompanyRepository.Get(production.CompanyID);
                                            if(company != null)
                                            {
                                                companies.Add(company.Name + " | " + production.Name);
                                            }
                                            sumItems += shopping.Quantity;
                                            sumTotal += (int)production.Price * shopping.Quantity;
                                            product_names +=
                                                $" {++counter}. Product Name :" +
                                                production.Name + " | " + " Quantity :" +
                                                production.Quantity + " | " + " Price :" +
                                                production.Price + " | " + " Company Name :" +       
                                                company.Name +
                                                "\n\n";
                                            _unitofwork.ProductRepository.Update(production);
                                            await _unitofwork.Save();
                                        }
                                    }

                                    await _unitofwork.ReceiptRepository.Add(new Receipt
                                    {
                                        UserId = user.Id,
                                        Amount = response.data.amount,
                                        TransactionDate = response.data.transaction_date,
                                        Currency = response.data.currency,
                                        ReferenceCode = response.data.reference,
                                        GatewayResponse = response.data.gateway_response,
                                        Channel = response.data.channel,
                                        CustomerFirstName = response.data.customer.first_name ?? user.FirstName,
                                        CustomerLastName = response.data.customer.last_name ?? user.LastName,
                                        CustomerEmail = response.data.customer.last_name ?? user.LastName,
                                        CardType = response.data.authorization.card_type,
                                        last4 = response.data.authorization.last4,
                                        ExpMonth = response.data.authorization.exp_month,
                                        ExpYear = response.data.authorization.exp_year,
                                        Bin = response.data.authorization.bin,
                                        Bank = response.data.authorization.bank,
                                        CountryCode = response.data.authorization.country_code,
                                        CustomerPhoneNumber = user.PhoneNumber,
                                        NumberOfItems = sumItems,
                                        PurchasedItems = product_names,
                                        Total = amount,
                                        OrderNumber = o_num
                                    });

                                    var generated_invoices = await _invoice.GenerateReceipt(user.Id, response.data.reference); 
                                    
                                    await _emailhandler.SendEmailAsync(user.Email,
                                        $"Payment Successful for {user.FirstName} {user.LastName}",
                                        $"<p>Congratulations, {user.FirstName} {user.LastName} your payment was successful with Refference Code: {response.data.reference} on {response.data.transaction_date}</p>" +
                                        $"<p>Order Status is currently on Payment Received.</p>" +
                                        $"You can log on to the app to check on the status of the order" +
                                        $"<p> Please find attached to this email your Payment Receipt: <a href = {generated_invoices}>Payment Receipt</a></p>");

                                    await _emailhandler.SendEmailAsync("txwebservicesghana@gmail.com",
                                        $"Payment Successful for {user.FirstName} {user.LastName}",
                                        $"<p>This email is to notify you of payment made by customer {user.FirstName} {user.LastName} with Refference Code: {response.data.reference} o {response.data.transaction_date}</p>" +
                                        $"Order Status is currently on Payment Received." +
                                        $"Any of the TX App administrators can log on to handle the status of this order." +
                                        $"<p> Please find attached to this email his/her Payment Receipt: <a href = {generated_invoices}>Payment Receipt</a></p>");
                                    if(shopping_carts != null)
                                    {
                                        foreach (var shopper in shopping_carts)
                                        {
                                            var the_product = await _unitofwork.ProductRepository.Get(shopper.ProductID);
                                            var the_company = await _unitofwork.CompanyRepository.Get(the_product.CompanyID);
                                            await _emailhandler.SendEmailAsync(the_company.Email,
                                                $"Product Purchase",
                                                $"<p>This email is to notify you of purchase made of {shopper.Quantity} item(s) of {the_product.Name} worth {the_product.Price}</p>" +
                                                $"Amounting to a total of {(int)the_product.Price * the_product.Quantity} on {response.data.transaction_date}" +
                                                $"Order Status is currently on Payment Received." +
                                                $"<p>You wil receive payment on your next settlement date.</a></p>");   

                                            //might send via mobile as well.
                                        }
                                    }
                                    await _unitofwork.Save();
                                    
                                    return response.message + " " + " transaction " + response.data.gateway_response;
                                }
                                throw new BusinessException("This card is not a saved card");
                            }
                            throw new BusinessException("The card is not reusable");
                        }
                        return response.message + " " + " transaction " + response.data.gateway_response;
                    }
                }

            }
            throw new BusinessException("There are no saved cards for this user");
        }
        public async Task<IList<PayStackDotNetSDK.Models.Transactions.Data>> RetreiveAllSuccessfulTransactions(string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if (user != null && await _usermanager.IsInRoleAsync(user, SD.Customer))
            {
                if (!await _usermanager.IsPhoneNumberConfirmedAsync(user) && !await _usermanager.IsEmailConfirmedAsync(user))
                {
                    throw new BusinessException("Sorry you must confirm email and phone number before making this transaction");
                }
                var connectionInstance = new PaystackTransaction(_configuration["Test_Paystack_api_key"]);
                var response = await connectionInstance.ListTransactions();
                IList<PayStackDotNetSDK.Models.Transactions.Data> successful_transactions = new List<PayStackDotNetSDK.Models.Transactions.Data>();
                var response_data = response.data;

                foreach (var successful in response_data)
                {
                    if (
                        successful.status == "success" &&
                        successful.gateway_response == "Successful" &&
                        successful.customer.first_name == user.FirstName &&
                        successful.customer.last_name == user.LastName &&
                        successful.customer.email == user.EmailForCard)
                    {
                        successful_transactions.Add(successful);
                    }
                }
                return successful_transactions;
            }
            throw new BusinessException("The user either doesnt exist or is not a customer");
        }
        public async Task<string> CreateSubAccount(string companyid,int charge_percentage, ShoppingCart[] shoppingcarts)
        {
            var company = await _unitofwork.CompanyRepository.Get(companyid);
            List<PayStackDotNetSDK.Models.CustomField> fields = new List<PayStackDotNetSDK.Models.CustomField>();
            foreach(var shoppingcart in shoppingcarts)
            {
                int counter = 0;
                var product = await _unitofwork.ProductRepository.Get(shoppingcart.ProductID);
                PayStackDotNetSDK.Models.CustomField custom = new PayStackDotNetSDK.Models.CustomField
                {
                    display_name = "Product Purchased",
                    variable_name = "Product",
                    value = $"{++counter}. Product Name: {product.Name} | Quantity Purchased: {shoppingcart.Quantity} / {product.Quantity} | Product Price: {product.Price}",
                };
                fields.Add(custom);
            }
            
            if (company != null)
            {
                var bank_code = string.Empty;
                var code = await ListBanks(company.SupportedBank);
                if (code.Contains("Bank_code"))
                {
                    var code_array = code.Split("|");
                    bank_code = code_array[1];
                }
                var connectionInstance = new PaystackSubAccount(_configuration["Test_Paystack_api_key"]);
                var response = await connectionInstance.CreateSubAccount(new SubAccountModel
                {
                    business_name = company.Name,
                    settlement_bank = code.Contains("Bank_code") ? bank_code : throw new BusinessException("This bank is not supported on our application"),
                    percentage_charge = charge_percentage,
                    account_number = company.AccountNumber,                      
                    primary_contact_email = company.Email,
                    primary_contact_phone = company.PhoneNumber,  
                    metadata =
                    {
                        custom_fields = fields
                    },
                    settlement_schedule = "weekly"
                });

                if (response.status)  
                {
                    return response.message + "|" + response.data.subaccount_code;
                }
                return response.message;
            }
            throw new BusinessException("The company is empty");
            
        }
        public async Task<string> ListBanks(string bankname)
        {
            var connectionInstance = new PaystackListedBanks(_configuration["Test_Paystack_api_key"]);
            var response = await connectionInstance.ListBanks();
            if (response.status)
            {
                foreach(var data in response.data)
                {
                    if(bankname == data.name || data.name.Contains(bankname))
                    {
                        return $"Bank_code|{data.code}|{data.name}";
                    }
                }
                throw new BusinessException("The bank you have added is not part of our supported banks");   
            }
            return response.message;
        }
        public async Task<string> Handlemultisplitpayments(IList<SubAccountEntity> sub, int sum_amount, string userId)
        {
            var user = await _usermanager.FindByIdAsync(userId);
            if(user == null)
            {
                throw new BusinessException("The user does not exist");
            }
            SplitEntity split = new SplitEntity
            {
                name = "Company & TX Spare Parts Percentage Split",  
                type = "percentage",
                currency = SD.Ghana_Cedis,
                subaccounts = sub.ToList()
            };
            var httpTransaction = _clientfactory.CreateClient();
            var httpClient = _clientfactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_configuration["Test_Paystack_api_key"]}");
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.BaseAddress = new Uri("https://api.paystack.co");   
            JsonContent content = JsonContent.Create(split);
            var url = "/split";
            HttpResponseMessage responsehttp = await httpClient.PostAsync(url, content);
            if(responsehttp.StatusCode == HttpStatusCode.OK)
            {
                var transaction_reference = Guid.NewGuid().ToString();
                string response_in_string_format = await responsehttp.Content.ReadAsStringAsync();
                HandleMultipleSplitResponse response = JsonConvert.DeserializeObject<HandleMultipleSplitResponse>(response_in_string_format);
                if (response.status)
                {
                    httpTransaction.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_configuration["Test_Paystack_api_key"]}");
                    httpTransaction.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpTransaction.BaseAddress = new Uri("https://api.paystack.co");   
                    var transaction_url = "/initialize";
                    var initialize_entity = new InitializeEntity
                    {
                        refference = transaction_reference,
                        amount = $"{sum_amount}",
                        email = user.Email,
                        currency = SD.Ghana_Cedis,
                        callback_url = $"{_configuration["AppUrl"]}/api/Customer/Customer/verifytransaction?userid={user.Id}&refferencode={transaction_reference}",
                        channels = new string[] { "card", "mobile_money" },
                        split_code = response.data.split_code
                    };
                    JsonContent initialize_json = JsonContent.Create(initialize_entity);
                    HttpResponseMessage transaction_response = await httpTransaction.PostAsync(transaction_url, initialize_json);
                    if(transaction_response.StatusCode == HttpStatusCode.OK)
                    {
                        string transaction_response_string = await transaction_response.Content.ReadAsStringAsync();
                        InitializeAuthorizationResponseModel initial_response =
                            JsonConvert.DeserializeObject<InitializeAuthorizationResponseModel>(transaction_response_string);
                        if (initial_response.status)
                        {
                            return initial_response.data.authorization_url + "|" + initial_response.data.reference;
                        }
                        return initial_response.message;
                    }
                    throw new BusinessException($"{transaction_response.StatusCode}");   
                    
                }
                return response.message;
            }
            throw new BusinessException($"{responsehttp.StatusCode} request was not successful");   
        }
        public async Task<string> CheckSubAccount(string businessname)
        {
            var connectionInstance = new PaystackSubAccount(_configuration["Test_Paystack_api_key"]);
            var response = await connectionInstance.ListSubAccounts();
            if (response.status)
            {
                var list_data = response.data;
                foreach(var list in list_data)
                {
                    if(list.business_name == businessname)
                    {
                        return list.subaccount_code + "|" + "correct";   
                    }
                }
                return response.message;
            }
            return response.message;
        }            
        //yyyy-mm-dd
        public async Task<string> HandleDisputes(string from, string to)
        {
            var httpDisputes = _clientfactory.CreateClient();
            var httpUploadUrl = _clientfactory.CreateClient();
            httpDisputes.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_configuration["Test_Paystack_api_key"]}");
            httpDisputes.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpDisputes.BaseAddress = new Uri("https://api.paystack.co");

            //list pending disputes
            var url = $"/dispute?from={from}&to={to}";
            var result = await httpDisputes.GetAsync(url);
            List<ListDisputeResponse.Data> pending_list = null;
            if (result.IsSuccessStatusCode)
            {
                string response_string = await result.Content.ReadAsStringAsync();
                if(response_string != null)
                {
                    var responseJson = JsonConvert.DeserializeObject<ListDisputeResponse>(response_string);
                    List<ResolveDisputeResponse.Data> resolved_disputes = null;
                    httpUploadUrl.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{_configuration["Test_Paystack_api_key"]}");
                    httpUploadUrl.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    httpUploadUrl.BaseAddress = new Uri("https://api.paystack.co/dispute");

                    if (responseJson.status)
                    {
                        foreach(var response in responseJson.data)
                        {
                            if(response.status == "awaiting-merchant-feedback")
                            {
                                pending_list.Add(response);
                                var response_url = $"/{response.id}/upload_url/upload_filename={response.transaction.reference}.pdf";
                                var upload_results = await httpUploadUrl.GetAsync(response_url);
                                if (upload_results.IsSuccessStatusCode)
                                {
                                    var upload_response = await upload_results.Content.ReadAsStringAsync();
                                    var uploaded_feedback = JsonConvert.DeserializeObject<UploadUrlResponse>(upload_response);
                                    if (uploaded_feedback.status)
                                    {
                                        var customer = await _usermanager.FindByEmailAsync(response.customer.email);
                                        var order = await _unitofwork.OrderRepository.GetFirstOrDefault(e => e.UserID == customer.Id);
                                        StreamContent strm = new StreamContent(new FileStream($"{_hostenvironment.WebRootPath}/Receipts/{customer.Id}/{order.OrderNumber}", FileMode.Open, FileAccess.Read));
                                        strm.Headers.ContentType = new MediaTypeWithQualityHeaderValue("application/pdf");
                                        var upload_res = await httpUploadUrl.PutAsync(uploaded_feedback.data.signedUrl, strm);
                                        if (upload_res.IsSuccessStatusCode)
                                        {
                                            var user = await _usermanager.FindByEmailAsync(response.customer.email);
                                            var shopcarts = await _unitofwork.ShoppingCartRepository.GetAll(e => e.UserID == user.Id);       
                                            var prod_name = string.Empty;
                                            
                                            var evidence = new EvidenceEntity
                                            {
                                                customer_email = user.Email,
                                                customer_name = user.FirstName + " " + user.LastName,
                                                customer_phone = user.PhoneNumber,
                                                delivery_address = user.PhysicalAdress ?? user.DigitalAddress       
                                            };
                                            foreach (var sh in shopcarts)
                                            {
                                                var product = await _unitofwork.ProductRepository.Get(sh.ProductID);
                                                prod_name += $"{product.Name},";
                                            }
                                            evidence.service_details = $"Claim for Buying Products : {prod_name.Remove(prod_name.Length)}";
                                            url = $"/{response.id}/evidence";
                                            JsonContent evidenceJson = JsonContent.Create(evidence);
                                            var req_response = await httpUploadUrl.PostAsync(url, evidenceJson);
                                            if (req_response.IsSuccessStatusCode)
                                            {
                                                var req_response_str = await req_response.Content.ReadAsStringAsync();
                                                var req_response_json = JsonConvert.DeserializeObject<EvidenceResponse>(req_response_str);
                                                if (req_response_json.status)
                                                {
                                                    url = $"/{response.id}/resolve";
                                                    //resolve issue here
                                                    var resolve_entity = new ResolveEntity
                                                    {
                                                        resolution = SD.dispute_declined,
                                                        message = "Merchant accepted",
                                                        uploaded_filename = response.transaction.reference ?? uploaded_feedback.data.fileName,
                                                        refund_amount = 0
                                                    };
                                                    JsonContent resolveJson = JsonContent.Create(resolve_entity);
                                                    var resolve_dispute = await httpUploadUrl.PutAsync(url, resolveJson);
                                                    if (resolve_dispute.IsSuccessStatusCode)
                                                    {
                                                        var resolve_dispute_str = await resolve_dispute.Content.ReadAsStringAsync();
                                                        var resolve_dispute_json = JsonConvert.DeserializeObject<ResolveDisputeResponse>(req_response_str);
                                                        if (resolve_dispute_json.status)
                                                        {
                                                            resolved_disputes.Add(resolve_dispute_json.data);
                                                        }
                                                    }
                                                    return $"{resolved_disputes.Count} disputes have been resolved between the dates from {from} to {to}";
                                                }
                                                return req_response_json.message;
                                            }
                                        }
                                    }
                                    return uploaded_feedback.message;

                                }
                            }
                            throw new BusinessException("Merchant is not awaiting merchant feedback");
                        }

                    }
                    return responseJson.message;
                }
                throw new BusinessException("The response string cannot be empty");

            }
            throw new BusinessException($"The result was not successful: {result.StatusCode}");
        }   
    } 
}
