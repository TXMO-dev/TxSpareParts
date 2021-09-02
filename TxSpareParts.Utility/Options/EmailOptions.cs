
using MailKit.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TxSpareParts.Utility.Options
{
    public class EmailOptions 
    {
        public EmailOptions()
        {
            Host_SecureSocketOptions = SecureSocketOptions.Auto;
        }

        public string Host_Address { get; set; }
        public int Host_Port { get; set; }
        public string Host_Username { get; set; }
        public string Host_Password { get; set; }
        public SecureSocketOptions Host_SecureSocketOptions { get; set; }
        public string Sender_Email { get; set; }
        public string Sender_Name { get; set; }
    }
}
