using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Entities
{
    public class UserImage
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string ImageUrl { get; set; }   
    }
}
