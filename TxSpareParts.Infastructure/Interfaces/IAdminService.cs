using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.CustomEntities;
using TxSpareParts.Core.Entities;
using TxSpareParts.Core.QueryFilter;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IAdminService
    {
        Task<string> LockoutServiceForChief(string id, SupervisorDTO assigntosupervisor);
        Task<string> LockoutServiceForAdmin(string id, SupervisorDTO assigntosupervisor);
        Task<string> UpdateService(string id, UpdateUserDTO user, string[] roles);
        Task<string> DeleteAdministratorForChief(string id);
        Task<string> DeleteStaff(string id,SupervisorDTO assigntosupervisor);
        Task<string> DeleteCustomer(string id);
        Task<PagedList<Order>> GetAllChiefOrdersService(StarredCompaniesQueryFIlter filter);
        Task<string> UpdateChiefOrderService(string userId, string orderId, Order order);
        Task<string> DeleteChiefOrderService(string userId, string orderId);
        Task<string> HandleDisputeChiefService(string userId, string from, string to);   
    }
}
