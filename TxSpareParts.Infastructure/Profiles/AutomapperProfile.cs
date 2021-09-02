using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Core.Entities;
using TxSpareParts.Infastructure.DTO;

namespace TxSpareParts.Infastructure.Profiles
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<RegisterDTO, ApplicationUser>();
            CreateMap<LoginDTO, ApplicationUser>();
        }
    }
}
