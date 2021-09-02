using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;

namespace TxSpareParts.Utility.Interfaces
{
    public interface IPaymentProcessingHandler
    {
        Task<string> InitializeTransaction(string subaccount_code, string userId, int amount, string currency);      
        Task<string> VerifyTransaction(string userId, string refference);
        Task<string> ChargeTransaction(string userId, string authorization_code, int amount);
        Task<IList<PayStackDotNetSDK.Models.Transactions.Data>> RetreiveAllSuccessfulTransactions(string userId);
        Task<string> CreateSubAccount(string companyid, int charge_percentage, ShoppingCart[] shoppingcarts);
        Task<string> ListBanks(string bankname);
        Task<string> Handlemultisplitpayments(IList<SubAccountEntity> sub, int sum_amount, string userId);
        Task<string> CheckSubAccount(string businessname);
        Task<string> HandleDisputes(string from, string to);
    }
}
