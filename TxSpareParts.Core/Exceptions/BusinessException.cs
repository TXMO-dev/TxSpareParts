using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Core.Exceptions
{
    public class BusinessException : Exception
    {
        public string _data { get; set; }
        public BusinessException()
        {

        }
        public BusinessException(string data) : base(data)
        {
            _data = data;
        }
    }
}
