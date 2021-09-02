using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<ApplicationUser>> GetAllSupervisorStaff(AllSupervisorDTO User, string Id);
    };
}
