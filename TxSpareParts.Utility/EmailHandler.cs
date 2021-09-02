using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TxSpareParts.Utility.Options;

namespace TxSpareParts.Utility
{
    public class EmailHandler : IEmailSender
    {
        private readonly EmailOptions _options;
        public EmailHandler(IOptions<EmailOptions> options)
        {
            _options = options.Value;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
             await Execute(email, subject, htmlMessage);
        }

        public async Task Execute(string to, string subject, string message)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_options.Sender_Email);
            if (!string.IsNullOrEmpty(_options.Sender_Name))
            {
                email.Sender.Name = _options.Sender_Name;
            }
            email.From.Add(email.Sender);
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html){ Text = message};

            using(var smtp = new SmtpClient())
            {
                await smtp.ConnectAsync(_options.Host_Address, Convert.ToInt32(_options.Host_Port), _options.Host_SecureSocketOptions);
                await smtp.AuthenticateAsync(_options.Host_Username, _options.Host_Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
        }
    }
}
