using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility.interfaces
{
    public interface IImageHandler
    {
        Task<string> CreateImage(string Id, string type, FileUpload file);
    }
}
